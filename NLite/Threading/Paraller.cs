using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using NLite.Internal;
using NLite.Threading.Internal;

namespace NLite.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public static class Paraller
    {
        private static readonly int ProcessorCount = Environment.ProcessorCount;
        private static readonly int ThreadsCount;

      
        static Paraller()
        {
            ThreadsCount = ProcessorCount * 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="body"></param>
        public static void For<T>(T[] array, Action<T> body)
        {
            Guard.NotNull(array, "array");
            Guard.NotNull(body, "body");

            For(0, array.Length, i => body(array[i]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="body"></param>
        public static void For<T>(T[] array, Func<T,bool> body)
        {
            Guard.NotNull(array, "array");
            Guard.NotNull(body, "body");

            For(0, array.Length, i => body(array[i]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="body"></param>
        public static void For(int fromInclusive, int toExclusive, Action<int> body)
        {
            Guard.NotNull(body, "body");

            var length = toExclusive - fromInclusive;
            if (length == 0)
                return;
          

            if (ThreadsCount == 1 || length == 1)
            {
                for (int i = fromInclusive; i < toExclusive; i++)
                    body(i);
                return;
            }

            int chunk = length / ThreadsCount;
            
            var tasks =  AllocateTasks(body, length, chunk);
            RunTasksByThreadPool(tasks);
            //RunTasksByThread(tasks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="body"></param>
        public static void For(int fromInclusive, int toExclusive, Func<int,bool> body)
        {
            Guard.NotNull(body, "body");
            var length = toExclusive - fromInclusive;
            if (length == 0)
                return;

            if (ThreadsCount == 1 || length == 1)
            {
                for (int i = fromInclusive; i < toExclusive; i++)
                    if (!body(i)) break;
                return;
            }

            int chunk = length / ThreadsCount;

            var tasks = AllocateTasks(body, length, chunk);
            RunTasksByThreadPool(tasks);
            //RunTasksByThread(tasks);
        }
      
        private static void RunTasksByThreadPool(TaskItem[] tasks)
        {
            var taskCount = tasks.Length;
            for (int theadIndex = 0; theadIndex < ThreadsCount - 1; theadIndex++)
            {
                var i = theadIndex;
                if (i == taskCount)
                    break;
                ThreadPool.QueueUserWorkItem(o => tasks[i].Run());
            }

            var taskId = ThreadsCount - 1;
            if(taskId < taskCount)
                tasks[taskId].Run();
           
        }

        //private static void RunTasksByThread(TaskItem[] tasks)
        //{
        //    for (int theadIndex = 0; theadIndex < ThreadsCount - 1; theadIndex++)
        //    {
        //        var i = theadIndex;
        //        new System.Threading.Thread( o=> tasks[i].Run()){ IsBackground = true }.Start();
        //    }

        //    tasks[ThreadsCount - 1].Run();
        //}

        private static TaskItem[] AllocateTasks(Delegate body, int length, int chunk)
        {
           
            var tasks = new List<TaskItem>();
            int remain = length;
            Barrier barrier = null;

            if (chunk == 0)
            {
                barrier = new Barrier(length);
                for (int i = 0; i < length; i++)
                {
                    tasks.Add(new TaskItem { Barrier = barrier, From = i ,To = i, Body = body});
                }
                return tasks.ToArray();
            }

            for (int i = 0; i < ThreadsCount; i++)
            {
                var task = new TaskItem { ThreadIndex = i + 1, Body = body, };
                task.From = i * chunk;
                if (i != ThreadsCount - 1)
                {
                    remain -= chunk;
                    task.To = task.From + chunk - 1;
                }
                else
                    task.To = task.From + remain - 1;

                if(task.To >= 0)
                    tasks.Add(task);
            }

            var taskCount = tasks.Count;
            barrier = new Barrier(taskCount);
            for (int i = 0; i < taskCount; i++)
            {
                var task = tasks[i];
                task.Barrier = barrier;
            }

            return tasks.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="body"></param>
        public static void ForEach<T>(IEnumerable<T> array, Action<T> body)
        {
            Guard.NotNull(array, "array");
            Guard.NotNull(body, "body");

            For<T>(array.ToArray(), body);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="body"></param>
        public static void ForEach<T>(IEnumerable<T> array, Func<T,bool> body)
        {
            Guard.NotNull(array, "array");
            Guard.NotNull(body, "body");

            For<T>(array.ToArray(), body);
        }
    }

    namespace Internal
    {

        class TaskItemAction : TaskItem
        {

            public new Func<int, bool> Body
            {
                get { return base.Body as Func<int,bool>; }
                set { base.Body = value; }
            }

            public override void Run()
            {
                var m = From;
                var n = To;

                try
                {
                    for (; m <= n; m++)
                        Body(m);
                }
                finally
                {
                }

                Barrier.Await();
            }

        }

        class TaskItemFunc : TaskItem
        {

            public new Func<int, bool> Body
            {
                get { return base.Body as Func<int, bool>; }
                set { base.Body = value; }
            }

            public override void Run()
            {
                var m = From;
                var n = To;

                try
                {
                    for (; m <= n; m++)
                    {
                        if (!Body(m))
                        {
                            break;
                        }
                    }
                }
                finally
                {
                }

                Barrier.Await();
            }

        }


        [DebuggerDisplay("ThreadIndex={ThreadIndex}, From={From},To={To}")]
        class TaskItem
        {
            public Barrier Barrier;
            public int ThreadIndex;
            public int From;
            public int To;
            public Delegate Body;

            public virtual void Run()
            {
                var m = From;
                var n = To;

                try
                {
                    for (; m <= n; m++)
                    {
                        var o = Body.DynamicInvoke(m);
                    }
                }
                finally
                {
                }

                Barrier.Await();
            }

        }



    
    }
}


