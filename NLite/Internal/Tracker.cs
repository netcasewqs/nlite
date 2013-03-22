using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Mini.Context;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Diagnostics;
using NLite.Threading;
using NLite.Mini.Lifestyle;
using System.Collections;
using NLite.Collections;

namespace NLite.Internal
{
    struct EmptyDisposeAction : IDisposable
    {
        public void Dispose() { }
    }

    struct ActionDisposable : IDisposable
    {
        private Object Owner;
        private Action Action;
        public ActionDisposable(Action action)
        {
            Action = action;
            Owner = null;
        }

        public ActionDisposable(object owner, Action action)
        {
            Owner = owner;
            Action = action;
        }

        public void Dispose()
        {
            if (Owner != null)
                ServiceRegistry.Current.UnRegister(Owner.GetType());
            if (Action != null)
                Action();
        }
    }



   

}
