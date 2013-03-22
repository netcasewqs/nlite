using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class FilePathResultAttribute : NavigationResultAttribute, IFilePathResult
    {
        string IFilePathResult.Path { get { return null; } }
        public string ContentType { get; set; }
        public string FileDownloadName { get; set; }
        public FilePathResultAttribute() : base(ActionResultTypes.FilePath) { }
    }
}
