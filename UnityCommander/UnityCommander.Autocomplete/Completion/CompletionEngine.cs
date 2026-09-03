
using Newtonsoft.Json.Linq;
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;
using UnityCommander.Autocomplete.Input;
using UnityCommander.Autocomplete.Tokenization;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Autocomplete.Completion
{
    public sealed class CompletionEngine : ICompletionEngine, IDiagnosticReporter
    {
        private readonly ITokenRegistry _tokenRegistry;
        private readonly IEnumerable<ICompletionProvider> _providers;
        private readonly ILogger? _logger;
        
        public string Name => "completion.engine";
        public DiagnosticCardinality Cardinality
        => DiagnosticCardinality.Single;

        public CompletionEngine(
                  ITokenRegistry tokenRegistry,
                  IEnumerable<ICompletionProvider> providers,
                  IDiagnosticRegistry diagnostic,
                  LoggerCreator? loggerCreator = null)
        {
            diagnostic.Register(this);
            _tokenRegistry = tokenRegistry;
            _providers = providers;
            _logger = loggerCreator?.For<CompletionEngine>(LogScope.Runtime);
        }

        public CompletionResult GetCompletions(
           InputState state,
           CliParseState analyze)
        {
            _logger.Debug(
                $"[ApplyCompletion] Input='{state.Text}', " +
                $"InputLength={state.Text?.Length}, " +
                $"CaretIndex={state.CaretPosition}, ");

            var providers = _providers
                .Where(p => p.CanHandle(analyze))
                .OrderByDescending(p => p.Priority)
                .ToList();

            var priority = providers.FirstOrDefault()?.Priority;

            //var final =
            // analyze.ExpectedNext == CompletionKind.Nothing ||
            // (analyze?.ExpectedValue?.Kind != CompletionKind.Flag &&
            //  analyze.AvailableFlags.Count > 0 &&
            //  analyze.AvailableFlags.Count == 0);

            var items = priority.HasValue
                ? providers
                    .Where(p => p.Priority == priority.Value)
                    .SelectMany(p => p.GetCompletions(analyze))
                    .Select(item => new CompletionItem
                    {
                        DisplayText = item.DisplayText,
                        InsertText = item.InsertText,
                        CaretOffset = item.CaretOffset,
                        EditFactory = s =>
                        {
                            var insertText = item.InsertText;
                            var replaceStart = analyze.ReplaceStart;
                            var replaceLength = analyze.ReplaceLength;

                            if (replaceStart > s.Text.Length)
                            {
                                replaceStart = s.Text.Length;
                                replaceLength = 0;

                                insertText = " " + insertText;
                            }

                            if (item.AppendSpace)
                                insertText += " ";

                            return new TextEdit(
                                replaceStart,
                                replaceLength,
                                analyze.CurrentToken,
                                insertText);
                        }
                    })
                    .ToList()
                : [];

            return new CompletionResult(items)
            {
                DefaultSelectedIndex =
                    items.Count > 0 ? items.Count - 1 : -1
            };
        }

        public TextEdit ApplyCompletion(InputState state, CompletionItem item)
        {
            if (item.EditFactory == null)
                throw new ArgumentNullException(nameof(item.EditFactory), "EditFactory не должен быть null.");
            return item.EditFactory(state);
        }

        public InputToken? GetTokenNearCaret(string text, int caretPosition)
            => _tokenRegistry.GetTokenNearCaret(text, caretPosition);

        public IReadOnlyList<InputToken> GetAllTokens()
            => _tokenRegistry.Tokens;

        public void Report(IDiagnosticWriter writer)
        {
            //writer.BeginTable("ActiveToken");

            //writer.Row("Input", _cliParseState?.ActiveToken?.Text);

            //writer.Row("CurrentToken", Status?.ActiveToken?.Text);
            //writer.Row("SemanticIndex", Status?.ActiveToken?.SemanticIndex);
            //writer.Row("Kind", Status?.ActiveToken?.Kind);
            //writer.Row("Status", Status?.ActiveToken?.Status);
            //writer.Row("Complete", Status?.ActiveToken?.IsComplete);

            //writer.EndTable();

            //writer.BeginTable("InputStatus");

            //writer.Row("CommandName", Status?.Command?.Name);

            //writer.Row("IsValidCommand", Status?.IsValidCommand);
            //writer.Row("VariantName", Status?.Variant?.Name);
            //writer.Row("ExpectedKind", Status?.ExpectedKind);
            //writer.Row("TokensCount", Status?.Tokens?.Count);

            //writer.Row("FlagUsage", _availableFlags?.Count);
            //writer.Row("ArgumentUsage", _availableArguments?.Count);

            //writer.EndTable();
        }
    }
}
