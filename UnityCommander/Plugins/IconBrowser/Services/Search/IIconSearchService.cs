using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IconBrowser.Services.Search
{
    public interface IIconSearchService
    {
        public Task SearchAsync(
             string query,
             IProgress<List<IconSearchResult>> progress,
             CancellationToken token);

        public Task RebuildIndexAsync();
    }
}