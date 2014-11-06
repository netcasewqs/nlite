using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Threading
{
    public interface IThreadTask
    {
        void Run();
    }

    public static class ThreadTask
    {
        public static readonly IThreadTask Empty = new EmptyTask();

        public static IThreadTask Make(Action action)
        {
            return new DelegateTask { Action = action };
        }

        private class EmptyTask : IThreadTask
        {
            public void Run() { }
        }

        private class DelegateTask : IThreadTask
        {
            public Action Action;

            public void Run()
            {
                if (Action != null)
                    Action();
                Action = null;
            }
        }
    }
}
