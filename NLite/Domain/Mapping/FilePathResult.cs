using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class FilePathResult : FileResult, IFilePathResult
    {
        public string Path { get; set; }

        public override string Type
        {
            get { return ActionResultTypes.FilePath; }
        }
    }
}
