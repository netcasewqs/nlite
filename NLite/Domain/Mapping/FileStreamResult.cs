using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class FileStreamResult:FileResult,IFileStreamResult
    {
        public override string Type
        {
            get { return ActionResultTypes.FileStream; }
        }

        public Stream Stream { get; set; }
    }
}
