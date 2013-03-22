//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace NLite.Threading
//{
//    public enum ThreadState
//    {
//        NotStarted,
//        Running,
//        Suspended,
//        Stopped,
//    }

//    public class ThreadWrapper:BooleanDisposable
//    {
//        private System.Threading.Thread InnerThread;
//        private ResetEvent mutex;

//        public string Name { get; private set; }
//        public int Id { get; private set; }
//        public IThreadTask Task { get; private set; }
//        public ThreadState State { get; private set; }


//        public ThreadWrapper(string name, IThreadTask task)
//        {
//            if (task == null)
//                throw new ArgumentNullException("task");

//            Task = task;
//            InnerThread = new System.Threading.Thread(() => task.Run()) { Name = name };
//            mutex = new ResetEvent(false);
//            State = ThreadState.NotStarted;
//        }


//        public void Start()
//        {
//            State = ThreadState.Running;
//            InnerThread.Start();

//        }

//        public void Suspend()
//        {
//            if (State != ThreadState.Running)
//                throw new InvalidOperationException("The thread has doest started.");

//            State = ThreadState.Suspended;
//            mutex.Wait();
//        }

//        public void Resume()
//        {
//            if(State != ThreadState.Suspended)
//                throw new InvalidOperationException("The thread has doest suspended.");

//            State = ThreadState.Running;
//            mutex.Set();
//        }

//        public void Join()
//        {
//            try
//            {
//                if (State == ThreadState.Suspended)
//                    Resume();

//                if (State == ThreadState.Running)
//                    InnerThread.Join();
//            }
//            finally
//            {
//                State = ThreadState.Stopped;
//            }
//        }

//        public void Abort()
//        {
//            try
//            {
//                if (State == ThreadState.Suspended)
//                    Resume();

//                if (State == ThreadState.Running)
//                    InnerThread.Abort();
//            }
//            finally
//            {
//                State = ThreadState.Stopped;
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//                Abort();
//        }

//    }
//}
