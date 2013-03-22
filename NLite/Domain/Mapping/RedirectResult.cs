using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class RedirectResult:IRedirectResult
    {
        public string Url { get; set; }

        public string Type
        {
            get { return ActionResultTypes.Redirect; }
        }
    }
}
