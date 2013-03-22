using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class FileContentResult:FileResult,IFileContentResult
    {
        public byte[] Content { get; set; }

        public override string Type
        {
            get { return ActionResultTypes.Content; }
        }
    }
}
