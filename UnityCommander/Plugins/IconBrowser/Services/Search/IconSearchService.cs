
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IconBrowser.Services.Search
{
    public class IconSearchService : IIconSearchService
    {
        private readonly IconSearchIndex _index;
        private readonly IconIndexBuilder _builder;
        private readonly string _rootPath;

        public IconSearchService(
            IconSearchIndex index,
            IconIndexBuilder builder,
            string rootPath)
        {
            _index = index;
            _builder = builder;
            _rootPath = rootPath;
        }

        public async Task SearchAsync(
            string query,
            IProgress<List<IconSearchResult>> progress,
            CancellationToken token)
        {
            var batchSize = 500;

            var all = _index.SearchRaw(query); // НЕ IEnumerable

            foreach (var batch in all.Chunk(batchSize))
            {
                token.ThrowIfCancellationRequested();

                var result = batch.ToList();

                if (result.Count > 0)
                    progress.Report(result);

                await Task.Yield(); // даём UI дышать
            }
        }

        public Task RebuildIndexAsync()
        {
            var data = _builder.Build(_rootPath);
            _index.Set(data);
            return Task.CompletedTask;
        }
    }
}
