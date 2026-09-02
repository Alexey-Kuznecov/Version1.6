
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Models;
using UnityCommander.Commands.Parsing;
using UnityCommander.Search.Filtering;
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

            if (!TryGetSearchArguments(
                    args,
                    context.Output,
                    out var searchArgs))
            {
                return;
            }

            var request = BuildSearchRequest(searchArgs);

            await ExecuteSearchAsync(
                request,
                context.Output,
                cancellationToken);
        }

        private async Task ExecuteSearchAsync(
            SearchRequest request,
            IConsoleOutput output,
            CancellationToken cancellationToken)
        {
            using var activity = output.StartActivity("Searching...");

            var stopwatch = Stopwatch.StartNew();

            var progress = new Progress<SearchProgress>(metrics =>
            {
                activity.Update(state =>
                {
                    state.Processed = metrics.Processed;
                    state.Found = metrics.Found;
                    state.Skipped = metrics.Skipped;
                    state.Elapsed = stopwatch.Elapsed;
                });
            });

            request.Progress = progress;

            var count = 0;

            await foreach (var result in _searchService.Search(
                request,
                cancellationToken))
            {
                if (result is not FileSearchResult fileResult)
                    continue;

                output.WriteLine(fileResult.Item.Path);

                count++;
            }
        }

        private SearchRequest BuildSearchRequest(
            SearchArguments args)
        {
            return new SearchRequest
            {
                Query = args.Query,

                Scope = new SearchScope
                {
                    Paths = [args.Path]
                },

                Filters = args.Filters,

                Matcher = args.Query.Contains('*') || args.Query.Contains('?')
                    ? new WildcardSearchMatcher()
                    : new NameSearchMatcher()
            };
        }

        private bool TryGetSearchArguments(
            IArgumentCollection args,
            IConsoleOutput output,
            out SearchArguments result)
        {
            result = default!;

            var path = args.GetAt(0);

            if (string.IsNullOrWhiteSpace(path))
            {
                output.WriteLine("Path is not specified.");
                return false;
            }

            var query = args.GetAt(1);

            if (!TryParseFilters(args, output, out var filters))
                return false;

            result = new SearchArguments(
                path,
                query,
                filters);

            return true;
        }

        private bool TryParseFilters(
             IArgumentCollection args,
             IConsoleOutput output,
             out IReadOnlyList<ISearchFilter> filters)
        {
            var result = new List<ISearchFilter>();

            if (TryCreateExtensionFilter(args, out var extensionFilter))
                result.Add(extensionFilter);

            if (!TryCreateDateFilters(args, output, result))
            {
                filters = [];
                return false;
            }

            if (!TryCreateSizeFilters(args, output, result))
            {
                filters = [];
                return false;
            }

            filters = result;
            return true;
        }

        private bool TryCreateSizeFilters(
            IArgumentCollection args,
            IConsoleOutput output,
            List<ISearchFilter> filters)
        {
            if (args.GetString("size-min") is { } minValue)
            {
                if (!TryParseSize(minValue, out var minSize))
                {
                    output.WriteLine($"Invalid size: {minValue}");
                    return false;
                }

                filters.Add(
                    new SizeSearchFilter(
                        SizeComparison.GreaterThanOrEqual,
                        minSize));
            }

            if (args.GetString("size-max") is { } maxValue)
            {
                if (!TryParseSize(maxValue, out var maxSize))
                {
                    output.WriteLine($"Invalid size: {maxValue}");
                    return false;
                }

                filters.Add(
                    new SizeSearchFilter(
                        SizeComparison.LessThanOrEqual,
                        maxSize));
            }

            if (args.GetString("size") is { } value)
            {
                if (!TryParseSize(value, out var size))
                {
                    output.WriteLine($"Invalid size: {value}");
                    return false;
                }

                filters.Add(
                    new SizeSearchFilter(
                        SizeComparison.Equal,
                        size));
            }

            return true;
        }

        private static bool TryParseSize(
            string value,
            out long bytes)
        {
            bytes = 0;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim();

            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (value.EndsWith("KB", StringComparison.OrdinalIgnoreCase))
            {
                return long.TryParse(
                    value[..^2],
                    out var number)
                    && TryMultiply(number, KB, out bytes);
            }

            if (value.EndsWith("MB", StringComparison.OrdinalIgnoreCase))
            {
                return long.TryParse(
                    value[..^2],
                    out var number)
                    && TryMultiply(number, MB, out bytes);
            }

            if (value.EndsWith("GB", StringComparison.OrdinalIgnoreCase))
            {
                return long.TryParse(
                    value[..^2],
                    out var number)
                    && TryMultiply(number, GB, out bytes);
            }

            return long.TryParse(value, out bytes);
        }

        private static bool TryMultiply(
            long value,
            long multiplier,
            out long result)
        {
            if (value < 0 || value > long.MaxValue / multiplier)
            {
                result = 0;
                return false;
            }

            result = value * multiplier;
            return true;
        }

        private bool TryCreateDateFilters(
            IArgumentCollection args,
            IConsoleOutput output,
            List<ISearchFilter> filters)
        {
            if (!TryAddDateFilter(
                    args.GetString("created-after"),
                    DateField.Creation,
                    DateComparison.After,
                    filters,
                    output))
            {
                return false;
            }

            if (!TryAddDateFilter(
                 args.GetString("created-before"),
                 DateField.Creation,
                 DateComparison.Before,
                 filters,
                 output))
            {
                return false;
            }

            if (!TryAddDateFilter(
                    args.GetString("modified-after"),
                    DateField.LastWrite,
                    DateComparison.After,
                    filters,
                    output))
            {
                return false;
            }

            if (!TryAddDateFilter(
                args.GetString("modified-before"),
                DateField.LastWrite,
                DateComparison.Before,
                filters,
                output))
            {
                return false;
            }

            return true;
        }

        private bool TryAddDateFilter(
            string? value,
            DateField field,
            DateComparison comparison,
            List<ISearchFilter> filters,
            IConsoleOutput output)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!DateTime.TryParse(value, out var date))
            {
                output.WriteError(
                    $"Invalid date: '{value}'.");
                return false;
            }

            filters.Add(
                new DateSearchFilter(
                    field,
                    comparison,
                    date));

            return true;
        }

        private bool TryCreateExtensionFilter(
            IArgumentCollection args,
            out ISearchFilter? filter)
        {
            var extensions = args.GetStrings("extensions");

            if (extensions.Count == 0)
            {
                filter = null;
                return false;
            }

            filter = new ExtensionSearchFilter(extensions);
            return true;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
