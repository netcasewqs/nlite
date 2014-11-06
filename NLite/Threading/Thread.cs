using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NLite.Threading
{
    /// <summary>
    /// 线程状态
    /// </summary>
    public enum ThreadState
    {
        /// <summary>
        /// 未启动
        /// </summary>
        NotStarted,
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        /// <summary>
        /// 挂起
        /// </summary>
        Suspended,
        /// <summary>
        /// 停止
        /// </summary>
        Stopped,
    }

    /// <summary>
    /// 线程类
    /// </summary>
    public sealed class SmartThread : BooleanDisposable
    {
        private System.Threading.Thread InnerThread;
        private ResetEvent mutex;

        /// <summary>
        /// 得到线程名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 线程Id
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 线程任务
        /// </summary>
        public IThreadTask Task { get; private set; }

        /// <summary>
        /// 线程状态
        /// </summary>
        public ThreadState State { get; private set; }


        /// <summary>
        /// 创建线程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="task"></param>
        public SmartThread(IThreadTask task, string name = null, bool isBackground = true, ApartmentState apartmentState = ApartmentState.MTA)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            Task = task;
            InnerThread = new System.Threading.Thread(() => task.Run()) { Name = name, IsBackground = isBackground };
            InnerThread.SetApartmentState(apartmentState);

            mutex = new ResetEvent(false);
            State = ThreadState.NotStarted;
        }

        /// <summary>
        /// 创建线程
        /// </summary>
        /// <param name="action"></param>
        /// <param name="threadName"></param>
        public SmartThread(Action action, string threadName = null, bool isBackground = true, ApartmentState apartmentState = ApartmentState.MTA) : this(ThreadTask.Make(action), threadName,isBackground,apartmentState) { }

        /// <summary>
        /// 启动线程
        /// </summary>
        public void Start()
        {
            State = ThreadState.Running;
            InnerThread.Start();

        }

        /// <summary>
        /// 挂起线程
        /// </summary>
        public void Suspend()
        {
            if (State != ThreadState.Running)
                throw new InvalidOperationException("The thread has doest started.");

            State = ThreadState.Suspended;
            mutex.Wait();
        }

        /// <summary>
        /// 唤醒线程
        /// </summary>
        public void Resume()
        {
            if (State != ThreadState.Suspended)
                throw new InvalidOperationException("The thread has doest suspended.");

            State = ThreadState.Running;
            mutex.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Join()
        {
            try
            {
                if (State == ThreadState.Suspended)
                    Resume();

                if (State == ThreadState.Running)
                    InnerThread.Join();
            }
            finally
            {
                State = ThreadState.Stopped;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Abort()
        {
            try
            {
                if (State == ThreadState.Suspended)
                    Resume();

                if (State == ThreadState.Running)
                    InnerThread.Abort();
            }
            finally
            {
                State = ThreadState.Stopped;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Abort();
        }

    }
}
