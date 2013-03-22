using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class FileContentResultAttribute : NavigationResultAttribute, IFileContentResult
    {
        byte[] IFileContentResult.Content { get { return null; } }
        public string ContentType { get; set; }
        public string FileDownloadName { get; set; }
        public FileContentResultAttribute() : base(ActionResultTypes.FileContent) { }
    }
}
