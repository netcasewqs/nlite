using System;
using System.Collections.Generic;
using NLite.Mini.Context;
using NLite.Threading;
using System.Threading;
using System.Collections;

namespace NLite.Mini.Lifestyle
{
    /// <summary>
    /// 线程生命周期管理器
    /// </summary>
    public class ThreadLifestyleManager : LifestyleManagerAdapter
    {
        [ThreadStatic]
        private static object instance;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public override object Get(IComponentContext ctx)
        {
            if (instance == null)
                instance = base.Get(ctx);
            return instance;
        }


    }

    /// <summary>
    /// 
    /// </summary>
    public class ThreadLifestyleManager2 : LifestyleManagerAdapter
    {

        private readonly string threadId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadLifestyleManager"/> class.
        /// </summary>
        public ThreadLifestyleManager2()
        {
            this.threadId = Thread.CurrentThread.ManagedThreadId.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public override object Get(IComponentContext ctx)
        {
            var componentKey = threadId + "_" + ctx.Component.GetHashCode();
            object result = Local.Get(componentKey);

            if (result == null)
            {
                result = base.Get(ctx);
                Local.Set(componentKey, result);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            
            base.Dispose(disposing);
            if (disposing)
            {
                Local._dataContext.Data.Clear();
            }
        }

    }
}
