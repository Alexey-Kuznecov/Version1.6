using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Core.IO;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Operation
{
    public class CopyReportCollector
    {
        private readonly List<CopyInfoModel> copiedFiles = new();
        private readonly List<CopyInfoModel> skippedFiles = new();

        public void Clear()
        {
            copiedFiles.Clear();
            skippedFiles.Clear();
        }

        public CopyInfoModel AddCopied(CopyInfo info)
        {
            var model = new CopyInfoModel
            {
                Name = info.Name,
                Source = info.Source,
                Destination = info.Destination
            };
            copiedFiles.Add(model);
            return model;
        }

        public CopyInfoModel AddSkipped(CopyInfo info)
        {
            var model = new CopyInfoModel
            {
                Name = info.Name,
                Source = info.Source,
            };
            skippedFiles.Add(model);
            return model;
        }

        public IReadOnlyList<CopyInfoModel> GetCopiedFiles() => copiedFiles.AsReadOnly();
        public IReadOnlyList<CopyInfoModel> GetSkippedFiles() => skippedFiles.AsReadOnly();
    }
}
