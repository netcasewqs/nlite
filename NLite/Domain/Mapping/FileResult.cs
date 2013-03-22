using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public abstract class FileResult:IFileResult
    {
        public string ContentType { get; set; }
        public string FileDownloadName { get; set; }
        public abstract string Type { get; }
    }
}
