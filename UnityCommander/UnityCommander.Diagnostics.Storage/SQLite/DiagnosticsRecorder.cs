
using System.Threading.Channels;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Storage.Serialization;

namespace UnityCommander.Diagnostics.Storage;

public sealed class DiagnosticsRecorder : IDiagnosticsRecorder
{
    private sealed record PendingRecord(
        Guid SessionId,
        DiagnosticRecord Record);

    private readonly IDiagnosticsStorage _storage;
    private readonly DiagnosticJsonSerializer _serializer;
    private readonly DiagnosticsStorageOptions _options;

    private readonly Channel<PendingRecord> _queue;

    private readonly SemaphoreSlim _lifecycleLock = new(1, 1);
    private readonly SemaphoreSlim _flushLock = new(1, 1);

    private readonly CancellationTokenSource _writerCancellation = new();

    private readonly object _stateLock = new();

    private DiagnosticSession? _currentSession;

    private Task? _writerTask;

    private List<StoredDiagnosticRecord>? _retryBatch;

    private bool _acceptingRecords;
    private bool _hasStarted;

    private int _disposeStarted;

    private long _droppedRecordCount;

    private Exception? _lastStorageError;

    public DiagnosticsRecorder(IDiagnosticsStorage storage)
    {
        _storage = storage;

        _serializer = new DiagnosticJsonSerializer();

        _options = new DiagnosticsStorageOptions();
        _options.Validate();

        _queue = Channel.CreateBounded<PendingRecord>(
            new BoundedChannelOptions(_options.QueueCapacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            });
    }

    public DiagnosticSession? CurrentSession =>
        Volatile.Read(ref _currentSession);

    public long DroppedRecordCount =>
        Interlocked.Read(ref _droppedRecordCount);

    public Exception? LastStorageError =>
        Volatile.Read(ref _lastStorageError);

    public async ValueTask<DiagnosticSession> StartSessionAsync(
        DiagnosticSessionInfo info,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(info);

        ArgumentException.ThrowIfNullOrWhiteSpace(info.Name);

        await _lifecycleLock.WaitAsync(cancellationToken);

        try
        {
            ThrowIfDisposed();

            if (_hasStarted)
            {
                throw new InvalidOperationException(
                    "A diagnostic session has already been started.");
            }

            await _storage.InitializeAsync(cancellationToken);

            var session = new DiagnosticSession(
                Guid.NewGuid(),
                info.Name,
                DateTimeOffset.UtcNow,
                null,
                info.ApplicationVersion,
                info.GitCommit,
                info.Configuration);

            await _storage.CreateSessionAsync(
                session,
                cancellationToken);

            lock (_stateLock)
            {
                Volatile.Write(ref _currentSession, session);

                _acceptingRecords = true;
            }

            _hasStarted = true;

            _writerTask = Task.Run(
                () => WriterLoopAsync(_writerCancellation.Token));

            return session;
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    public bool TryRecord(DiagnosticRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (Volatile.Read(ref _disposeStarted) != 0)
        {
            return false;
        }

        lock (_stateLock)
        {
            if (!_acceptingRecords || _currentSession is null)
            {
                return false;
            }

            var pendingRecord = new PendingRecord(
                _currentSession.Id,
                record);

            if (_queue.Writer.TryWrite(pendingRecord))
            {
                return true;
            }

            Interlocked.Increment(ref _droppedRecordCount);

            return false;
        }
    }

    public async ValueTask FlushAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_hasStarted)
        {
            return;
        }

        await _flushLock.WaitAsync(cancellationToken);

        try
        {
            await FlushPendingAsync(cancellationToken);
        }
        finally
        {
            _flushLock.Release();
        }
    }

    public async ValueTask StopSessionAsync(
        CancellationToken cancellationToken = default)
    {
        await _lifecycleLock.WaitAsync(cancellationToken);

        try
        {
            DiagnosticSession? session = CurrentSession;

            if (session is null || session.FinishedAt.HasValue)
            {
                return;
            }

            lock (_stateLock)
            {
                _acceptingRecords = false;
            }

            _writerCancellation.Cancel();

            if (_writerTask is not null)
            {
                await _writerTask.ConfigureAwait(false);
            }

            DateTimeOffset completedAt = DateTimeOffset.UtcNow;

            await _storage.CompleteSessionAsync(
                session.Id,
                completedAt,
                cancellationToken);

            Volatile.Write(
                ref _currentSession,
                session with
                {
                    FinishedAt = completedAt
                });
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    private async Task WriterLoopAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(
                _options.FlushInterval);

            while (await timer.WaitForNextTickAsync(
                cancellationToken))
            {
                try
                {
                    await FlushAsync(cancellationToken);

                    Volatile.Write(
                        ref _lastStorageError,
                        null);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    Volatile.Write(
                        ref _lastStorageError,
                        exception);
                }
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            await FlushAsync(CancellationToken.None);
        }
    }

    private async Task FlushPendingAsync(
        CancellationToken cancellationToken)
    {
        // Сначала повторяем последнюю неудачную запись.
        if (_retryBatch is not null)
        {
            await _storage.WriteBatchAsync(
                _retryBatch,
                cancellationToken);

            _retryBatch = null;

            Volatile.Write(
                ref _lastStorageError,
                null);
        }

        while (true)
        {
            var batch = new List<StoredDiagnosticRecord>(
                _options.BatchSize);

            while (
                batch.Count < _options.BatchSize &&
                _queue.Reader.TryRead(out PendingRecord? pending))
            {
                StoredDiagnosticRecord storedRecord =
                    ConvertToStoredRecord(pending);

                batch.Add(storedRecord);
            }

            if (batch.Count == 0)
            {
                break;
            }

            // Сохраняем пакет до завершения SQL-транзакции.
            // Если запись завершится ошибкой, пакет останется
            // в памяти для следующей попытки.
            _retryBatch = batch;

            await _storage.WriteBatchAsync(
                _retryBatch,
                cancellationToken);

            _retryBatch = null;

            Volatile.Write(
                ref _lastStorageError,
                null);
        }
    }

    private StoredDiagnosticRecord ConvertToStoredRecord(
        PendingRecord pending)
    {
        DiagnosticRecord record = pending.Record;

        return new StoredDiagnosticRecord(
            Guid.NewGuid(),
            pending.SessionId,
            record.Kind,
            record.Timestamp,
            record.Source,
            record.Name,
            record.PluginId,
            record.Duration?.Ticks,
            _serializer.Serialize(record.Payload));
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            Volatile.Read(ref _disposeStarted) != 0,
            this);
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposeStarted, 1) != 0)
        {
            return;
        }

        try
        {
            await StopSessionAsync(CancellationToken.None);
        }
        finally
        {
            await _storage.DisposeAsync();

            _writerCancellation.Dispose();
            _lifecycleLock.Dispose();
            _flushLock.Dispose();
        }
    }
}