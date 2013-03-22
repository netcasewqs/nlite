using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using NLite.Net;
using System.Reflection;
using NLite.Collections;

namespace NLite.Net
{
    public interface IResourceResult
    {
        void Execute(IHttpContext context);
    }
}

