
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview;

public sealed class PreviewRegistry : IPreviewRegistry
{
    private readonly object _sync = new();

    private readonly Dictionary<string, Registration<IPreviewProvider>> _providers =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, Registration<IPreviewPresenter>> _presenters =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<IPreviewProvider> Providers
    {
        get
        {
            lock (_sync)
            {
                return _providers.Values
                    .Select(x => x.Value)
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }
    }

    public IReadOnlyList<IPreviewPresenter> Presenters
    {
        get
        {
            lock (_sync)
            {
                return _presenters.Values
                    .Select(x => x.Value)
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }
    }

    public void RegisterProvider(string ownerId, IPreviewProvider provider)
    {
        ValidateRegistration(ownerId, provider?.Id);

        lock (_sync)
        {
            if (_providers.ContainsKey(provider.Id))
                throw new InvalidOperationException(
                    $"Preview provider '{provider.Id}' is already registered.");

            _providers.Add(
                provider.Id,
                new Registration<IPreviewProvider>(ownerId, provider));
        }
    }

    public void RegisterPresenter(string ownerId, IPreviewPresenter presenter)
    {
        ValidateRegistration(ownerId, presenter?.Id);

        lock (_sync)
        {
            if (_presenters.ContainsKey(presenter.Id))
                throw new InvalidOperationException(
                    $"Preview presenter '{presenter.Id}' is already registered.");

            _presenters.Add(
                presenter.Id,
                new Registration<IPreviewPresenter>(ownerId, presenter));
        }
    }

    public IPreviewPresenter? FindPresenter(PreviewResult result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));

        lock (_sync)
        {
            if (!string.IsNullOrWhiteSpace(result.PresenterId))
            {
                if (_presenters.TryGetValue(result.PresenterId, out var registration) &&
                    registration.Value.Kind == result.Content.Kind)
                {
                    return registration.Value;
                }

                return null;
            }

            return _presenters.Values
                .Select(x => x.Value)
                .Where(x => x.Kind == result.Content.Kind)
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault();
        }
    }

    public void Cleanup(string ownerId)
    {
        if (string.IsNullOrWhiteSpace(ownerId))
            return;

        lock (_sync)
        {
            var providerIds = _providers
                .Where(x => string.Equals(
                    x.Value.OwnerId,
                    ownerId,
                    StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Key)
                .ToArray();

            foreach (var id in providerIds)
                _providers.Remove(id);

            var presenterIds = _presenters
                .Where(x => string.Equals(
                    x.Value.OwnerId,
                    ownerId,
                    StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Key)
                .ToArray();

            foreach (var id in presenterIds)
                _presenters.Remove(id);
        }
    }

    private static void ValidateRegistration(string ownerId, string? id)
    {
        if (string.IsNullOrWhiteSpace(ownerId))
            throw new ArgumentException("Owner ID cannot be empty.", nameof(ownerId));

        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Registration ID cannot be empty.", nameof(id));
    }

    private sealed class Registration<T>
    {
        public string OwnerId { get; }

        public T Value { get; }

        public Registration(string ownerId, T value)
        {
            OwnerId = ownerId;
            Value = value;
        }
    }
}