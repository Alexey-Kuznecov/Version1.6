
using System;

namespace UnityCommander.Common.Layout
{
    public interface ILayoutContentFactory
    {
        object Create(Guid contentId, string path);
    }
}
