//using System;
//using System.Collections.Generic;

//namespace NLite.Threading
//{
//    public delegate void CancelCallback(object arg);

//    public interface IFuture : IAsyncResult
//    {
//        int Id { get; }
//        IAsyncResult AsyncResult { get; set; }
//        IFuture Parent { get; }
//        IEnumerable<IFuture> Childs { get; }

//        Exception Exception { get; set; }
//        Action<IFutureContext> CompletedHandler { get; }
//        bool IsSuccessed { get; }
//        bool Cancelled { get; }

//        bool Wait();
//        bool Wait(int millisecondsTimeout);

//        void Cancel();
//        void Cancel(CancelCallback cancelCallback, object arg);
//        void Set();
//        void Reset();

//        void Close();
//    }
//}
