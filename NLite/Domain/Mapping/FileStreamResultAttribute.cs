using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class FileStreamResultAttribute : NavigationResultAttribute, IFileStreamResult
    {
        Stream IFileStreamResult.Stream { get { return null; } }
        public string ContentType { get; set; }
        public string FileDownloadName { get; set; }
        public FileStreamResultAttribute() : base(ActionResultTypes.FileStream) { }
    }
}
