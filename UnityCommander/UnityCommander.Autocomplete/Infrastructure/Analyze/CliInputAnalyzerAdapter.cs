
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class CliInputAnalyzerAdapter //: ICliInputAnalyzer
    {
        private readonly CliInputAnalyzer _newAnalyzer;

        public CliInputAnalyzerAdapter(CliInputAnalyzer newAnalyzer)
        {
            _newAnalyzer = newAnalyzer;
        }

        public CliParseState Analyze(string text, int caretPosition)
        {
            var status = _newAnalyzer.Analyze(text, caretPosition);

            // Выбираем активный токен
            var activeToken = status.Tokens.FirstOrDefault(t => t.Status == TokenStatus.Editing)
                              ?? status.Tokens.LastOrDefault();

            // Определяем команду
            var command = status.Command;

            // Разбираем позиционные аргументы
            var positionalArgs = new List<ParsedArgument>();

            if (command != null && command.Variants.Any())
            {
                var variant = command.Variants.First();
                int argIndex = 0;

                foreach (var token in status.Tokens)
                {
                    if (token.Kind != TokenKind.PositionalArgument)
                        continue;

                    if (argIndex >= variant.Arguments.Count)
                        break;

                    var argumentDescriptor = variant.Arguments[argIndex];

                    var parsedArg = new ParsedArgument(argumentDescriptor, token.Text);
                    positionalArgs.Add(parsedArg);

                    argIndex++;
                }
            }

            // Разбираем флаги
            var flags = new List<ParsedFlag>();

            if (command != null && command.Variants.Any())
            {
                var variant = command.Variants.First();

                foreach (var token in status.Tokens)
                {
                    if (token.Kind != TokenKind.Flag)
                        continue;

                    IFlagDescriptor? flagDescriptor = null;

                    foreach (var flag in variant.Flags)
                    {
                        if (flag.Name == token.Text ||
                            (!string.IsNullOrEmpty(flag.ShortName) && flag.ShortName == token.Text))
                        {
                            flagDescriptor = flag;
                            break;
                        }
                    }

                    var parsedFlag = new ParsedFlag(flagDescriptor, null);
                    flags.Add(parsedFlag);
                }
            }

            var availableArguments = new List<SimplePositionalArgumentDescriptor>();

            if (command != null && command.Variants.Any())
            {
                var variant = command.Variants.First();
                int skipCount = positionalArgs.Count;

                for (int i = skipCount; i < variant.Arguments.Count; i++)
                {
                    if (variant.Arguments[i] is SimplePositionalArgumentDescriptor arg)
                    {
                        availableArguments.Add(arg);
                    }
                }
            }

            var availableFlags = new List<SimpleFlagDescriptor>();

            if (command != null && command.Variants.Any())
            {
                var variant = command.Variants.First();

                foreach (var flag in variant.Flags)
                {
                    bool alreadyUsed = false;

                    foreach (var parsedFlag in flags)
                    {
                        if (parsedFlag.Descriptor == flag)
                        {
                            alreadyUsed = true;
                            break;
                        }
                    }

                    if (flag.IsRepeatable || !alreadyUsed)
                    {
                        if (flag is SimpleFlagDescriptor simpleFlag)
                        {
                            availableFlags.Add(simpleFlag);
                        }
                    }
                }
            }

            return new CliParseState(
                command,
                positionalArgs,
                flags,
                availableArguments,
                availableFlags,
                MapPhaseToCompletionKind(status.ExpectedKind),
                positionalArgs.Count,
                null,
                replaceStart: activeToken?.Start ?? 0,
                replaceLength: activeToken?.Length ?? 0,
                partialValue: activeToken?.Text ?? ""
            );
        }

        private CompletionKind MapPhaseToCompletionKind(ExpectedKind phase)
        {
            return phase switch
            {
                ExpectedKind.Command => CompletionKind.Command,
                ExpectedKind.Variant => CompletionKind.Variant,
                ExpectedKind.Flag => CompletionKind.Flag,
                ExpectedKind.PositionalArgument => CompletionKind.PositionalArgument,
                ExpectedKind.FlagValue => CompletionKind.FlagValue,
                ExpectedKind.Nothing => CompletionKind.Nothing,
                _ => CompletionKind.Nothing
            };
        }
    }
}
