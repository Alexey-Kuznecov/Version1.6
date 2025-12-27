
using UnityCommander.Autocomplete.Input;
using UnityCommander.Autocomplete.Tokenization;

namespace UnityCommander.Autocomplete.Context.Resolution
{
    public class InputContextResolver : IInputContextResolver
    {
        public InputContext Resolve(TokenizationResult tokens)
        {
            // Проверка на null для избежания CS8602
            var tokenList = tokens.Tokens ?? Array.Empty<InputToken>();

            // Простейшая логика: если токен первый → команда, иначе аргумент
            if (tokenList.Count == 0 || tokenList.Count == 1)
            {
                var partial = tokenList.FirstOrDefault()?.CurrentValue ?? "";
                return new CommandNameContext(tokens, partial);
            }
            else
            {
                var tokensResult = tokens.Tokens ?? throw new InvalidOperationException("CurrentToken не может быть null"); ;
                InputToken currentToken = tokens.CurrentToken ?? new InputToken
                {
                    Start = tokens.CaretPosition,
                    Length = 0,
                    CurrentValue = string.Empty
                };

                return new ArgumentContext(
                    tokens,
                    commandName: tokensResult.FirstOrDefault()?.CurrentValue ?? string.Empty,
                    existingArguments: tokensResult.Skip(1).Select(t => t.CurrentValue ?? string.Empty).ToArray(),
                    partialArgument: currentToken.CurrentValue ?? throw new InvalidOperationException("currentToken не может быть null"),
                    replaceStart: currentToken.Start,
                    replaceLength: currentToken.Length);
            }
        }
    }
}
