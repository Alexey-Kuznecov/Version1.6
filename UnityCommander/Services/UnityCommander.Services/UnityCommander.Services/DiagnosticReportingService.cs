
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Reporting;
using UnityCommander.Logging.Contracts;

namespace UnityCommander.Services.Diagnostics
{
    public sealed class DiagnosticReportingService
       : IDiagnosticReportingService
    {
        private readonly IDiagnosticRegistry _registry;
        private readonly IDiagnosticsRecorder _recorder;
        private readonly ILogger _logger;

        public DiagnosticReportingService(
            IDiagnosticRegistry registry,
            IDiagnosticsRecorder recorder, AppLogger appLogger)
        {
            _registry = registry;
            _recorder = recorder;
            _logger = appLogger.For<DiagnosticReportingService>();
        }

        public bool Publish(DiagnosticEvent diagnosticEvent)
        {
            ArgumentNullException.ThrowIfNull(diagnosticEvent);

            if (_recorder.CurrentSession is null)
                return false;

            var components = new List<DiagnosticComponentReport>();

            var registrations = _registry
                .GetAll()
                .SelectMany(definition => definition.Instances)
                .Where(registration =>
                    string.Equals(
                        registration.Diagnostic.Subsystem,
                        diagnosticEvent.Subsystem,
                        StringComparison.OrdinalIgnoreCase));

            foreach (var registration in registrations)
            {
                var source = registration.Source;

                if (source is null)
                    continue;

                try
                {
                    var state = source.GetState();

                    components.Add(
                        new DiagnosticComponentReport(
                            registration.Diagnostic.Name,
                            state,
                            null));
                }
                catch (Exception exception)
                {
                    components.Add(
                        new DiagnosticComponentReport(
                            registration.Diagnostic.Name,
                            null,
                            $"{exception.GetType().Name}: {exception.Message}"));
                }
            }

            var payload = new DiagnosticReportPayload(
                diagnosticEvent.Payload,
                components);

            var record = new DiagnosticRecord(
               DiagnosticRecordKind.Report,
               diagnosticEvent.Timestamp,
               diagnosticEvent.Subsystem,
               diagnosticEvent.Name,
               payload,
               diagnosticEvent.PluginId,
               diagnosticEvent.Duration);

            var accepted = _recorder.TryRecord(record);

            _logger.Info(
                $"Reporting: {diagnosticEvent.Subsystem}.{diagnosticEvent.Name}, " +
                $"Session: {_recorder.CurrentSession?.Id}, " +
                $"Components: {components.Count}, " +
                $"Accepted: {accepted}");

            return accepted;
        }
    }
}
