
using System.Collections.Generic;
using UnityCommander.Search.Filtering;

namespace UnityCommander.Commands.Models
{
    public sealed record SearchArguments(
      string Path,
      string? Query,
      IReadOnlyList<ISearchFilter> Filters);
}
