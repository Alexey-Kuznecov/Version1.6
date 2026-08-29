
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Search.Models;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Commands
{
    [ConsoleCommand("search", "Поиск файлов")]
    public sealed class SearchCommand : IConsoleCommand
    {
        private readonly ICommandArgumentParser _parser;
        private readonly ISearchService _searchService;

        public string Name => "search";
        public string Description => "Поиск файлов";

        public CommandExecutionMode Mode
            => CommandExecutionMode.Background;

        public SearchCommand(
            ICommandArgumentParser parser,
            ISearchService searchService)
        {
            _parser = parser;
            _searchService = searchService;
        }

        public async Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            var output = context.Output;

            var path = args.GetAt(0);

            if (string.IsNullOrWhiteSpace(path))
            {
                output.WriteLine("Path is not specified.");
                return;
            }

            var request = new SearchRequest
            {
                Scope = new SearchScope
                {
                    Paths = [path]
                }
            };

            await foreach (var result in _searchService.Search(
                request,
                cancellationToken))
            {
                output.WriteLine(((FileSearchResult)result).Item.Path);
            }
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
