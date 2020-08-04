using System;
using Prism.Events;
using UnityCommander.Business;

namespace UnityCommander.Core
{
    public class CopyFilesEvent : PubSubEvent<FileCopyInfoModel>
    {
    }
}
