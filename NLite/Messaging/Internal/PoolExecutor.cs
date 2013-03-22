//using System;
//using System.Threading;

//namespace NLite.Messaging.Internal
//{
//    [Serializable]
//    class PoolExecutor : Executor
//    {
//        private int Status;

//        class ExecutorStatus
//        {
//            public const int Waiting = 0;
//            public const int Executing = 1;
//            public const int Exited = 2;
//        }

//        protected override void DoExecute()
//        {
//            int status = Interlocked.CompareExchange(ref Status, ExecutorStatus.Executing, ExecutorStatus.Waiting);

//            if (status == ExecutorStatus.Waiting)
//            {
//                Action doExecute = base.DoExecute;

//                ThreadPool.QueueUserWorkItem(state=>
//                {
//                    doExecute();

//                    if (Subject.Closed)
//                        Thread.VolatileWrite(ref Status, ExecutorStatus.Exited);
//                    else
//                    {
//                        Thread.VolatileWrite(ref Status, ExecutorStatus.Waiting);
//                        if (Subject.Queue.Count > 0)
//                            Execute();
//                    }
//                });
//            }
//        }
      
//    }
//}
