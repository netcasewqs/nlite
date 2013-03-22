//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Diagnostics;
//using NLite.Collections;

//namespace NLite.Threading
//{
//    public static partial class Future
//    {
//        private static IMap<int, IFuture> _pendings = new ConcurrentMap<int, IFuture>();
//        public static IFuture Current
//        {
//            get;
//            private set;
//        }

//        private static void Enqueue(IFuture future)
//        {
//            _pendings.Add(future.Id, future);
//            Current = future;
//        }

//        public static IFuture Create(Action handler)
//        {
//            var future = new SimpleFuture();
//            try
//            {
//                AsyncCallback callback = (state) =>
//                {
//                    try
//                    {
//                        if (future.AsyncResult != null)
//                            handler.EndInvoke(future.AsyncResult);
//                    }
//                    catch (Exception ex)
//                    {
//                        future.Exception = ex;
//                        ex.RaiseException();
//                    }
//                    future.Set();
//                };

//                Enqueue(future);
//                future.AsyncResult = handler.BeginInvoke(callback, null);
               
//            }
//            catch (Exception ex)
//            {
//                future.Exception = ex;
//                ex.RaiseException();
//            }
//            return future;
//        }

//        public static IFuture Create<T>(Action<T> handler, T arg)
//        {
//            var future = new SimpleFuture();
//            try
//            {
//                AsyncCallback callback = (state) =>
//                {
//                    try
//                    {
//                        if (future.AsyncResult != null)
//                            handler.EndInvoke(future.AsyncResult);
//                    }
//                    catch (Exception ex)
//                    {
//                        future.Exception = ex;
//                        ex.RaiseException();
//                    }
//                    future.Set();
//                };

//                Enqueue(future);
//                future.AsyncResult = handler.BeginInvoke(arg, callback, null);
               
//            }
//            catch (Exception ex)
//            {
//                future.Exception = ex;
//                ex.RaiseException();
//            }
//            return future;
//        }

//        public static IFuture<TResult> Create<TResult>(Func<TResult> handler)
//        {
//            var future = new AsyncFuture<TResult>();
//            try
//            {
//                AsyncCallback callback = (state) =>
//                {
//                    try
//                    {
//                        if (future.AsyncResult != null)
//                            future.Value = handler.EndInvoke(future.AsyncResult);
//                    }
//                    catch (Exception ex)
//                    {
//                        future.Exception = ex;
//                        ex.RaiseException();
//                    }
//                    future.Set();
//                };

//                Enqueue(future);
//                future.AsyncResult = handler.BeginInvoke(callback, null);
               
//            }
//            catch (Exception ex)
//            {
//                future.Exception = ex;
//                ex.RaiseException();
//            }
//            return future;
//        }

//        public static IFuture<TResult> Create<T, TResult>(Func<T, TResult> handler, T arg)
//        {
//            var future = new AsyncFuture<TResult>();
//            try
//            {
//                AsyncCallback callback = (state) =>
//                {
//                    try
//                    {
//                        if (future.AsyncResult != null)
//                            future.Value = handler.EndInvoke(future.AsyncResult);
//                    }
//                    catch (Exception ex)
//                    {
//                        future.Exception = ex;
//                        ex.RaiseException();
//                    }
//                    future.Set();
//                };

//                Enqueue(future);
//                future.AsyncResult = handler.BeginInvoke(arg, callback, null);
               
//            }
//            catch (Exception ex)
//            {
//                future.Exception = ex;
//            }
//            return future;
//        }

//        public static void WaitAll<TFuture>(this IEnumerable<TFuture> futures) where TFuture : IFuture
//        {
//            if (futures != null)
//                futures.ForEach(future => future.Wait());
//        }


//        public static void WaitAll()
//        {
//            if (_pendings.Count > 0)
//                WaitAll<IFuture>(_pendings.Values);
//        }
//    }

//    public static partial class Future
//    {
//        public class SimpleFuture : IFuture, IFutureContext
//        {

//            private static int _seed = 0;
//            private List<IFuture> _childs;

//            public SimpleFuture()
//            {
//                Parent = Future.Current;
//                _seed++;
//                Id = _seed;
//                _childs = new List<IFuture>(0);

//                var tmpParent = Parent as SimpleFuture;
//                if (tmpParent != null)
//                {
//                    tmpParent._childs.Add(this);
//                }
//            }

//            //public SimpleFuture(IAsyncResult ar)
//            //{
//            //    AsyncResult = ar;
//            //    _seed++;
//            //    Id = _seed;
//            //}

//            public int Id { get; private set; }

//            private IAsyncResult _ar;
//            private IFuture _innerFuture;
//            public IAsyncResult AsyncResult
//            {
//                get
//                {
//                    return _ar;
//                }
//                set
//                {
//                    if (value != _ar)
//                    {
//                        Trace.Assert(value != null, "value != null");
//                        if (_ar != null && _ar.AsyncWaitHandle != null && !_ar.AsyncWaitHandle.SafeWaitHandle.IsClosed)
//                        {
//                            try
//                            {
//                                _ar.AsyncWaitHandle.Close();
//                            }
//                            catch (Exception ex)
//                            {
//                                ex.RaiseException();
//                            }
//                        }
//                        _ar = value;

//                        _innerFuture = _ar as IFuture;
//                        if (_innerFuture != null)
//                        {
//                            SimpleFuture tmpFuture = _innerFuture as SimpleFuture;
//                            if (tmpFuture != null)
//                                tmpFuture.Parent = this;
//                        }
//                    }
//                }
//            }

//            public IFuture Parent { get; protected set; }
//            public IEnumerable<IFuture> Childs { get { return _childs.ToArray(); } }
//            public Action<IFutureContext> CompletedHandler { get; protected internal set; }

//            public virtual void Set()
//            {
//                if (Parent != null)
//                {
//                    Parent.Set();
//                    return;
//                }

//                //if (IsCompleted)
//                //    return;

//                if (AsyncResult == null || AsyncResult.AsyncWaitHandle == null)
//                {
//                    IsCompleted = true;
//                    return;
//                }

//                var mre = AsyncResult.AsyncWaitHandle as ManualResetEvent;
//                if (mre != null)
//                {
//                    IsCompleted = true;
//                    if (!Cancelled)
//                        OnCompleted();
//                    else
//                    {
//                        if (cancelHandler.HasValue)
//                            cancelHandler.Value.Invoke();
//                    }

//                    if (!mre.SafeWaitHandle.IsClosed)
//                    {
//                        mre.Set();
//                    }
//                }
//            }

//            public virtual void Reset()
//            {
//                if (AsyncResult == null || AsyncResult.AsyncWaitHandle == null)
//                {
//                    IsCompleted = false;
//                    return;
//                }

//                var mre = AsyncResult.AsyncWaitHandle as ManualResetEvent;
//                if (mre != null && !mre.SafeWaitHandle.IsClosed)
//                {
//                    IsCompleted = false;
//                    mre.Reset();
//                }
//            }

//            protected virtual void OnCompleted()
//            {
//                if (CompletedHandler != null)
//                {
//                    CompletedHandler.Send(this);
//                    CompletedHandler = null;
//                }
//                _pendings.Remove(this.Id);
//            }

//            private Exception exception;
//            public virtual Exception Exception
//            {
//                get
//                {
//                    if (exception == null)
//                    {
//                        if (_innerFuture != null)
//                            return _innerFuture.Exception;
//                    }
//                    return exception;
//                }
//                set
//                {
//                    if (value != null)
//                    {
//                        exception = value;
//                        Set();
//                    }
//                }
//            }

//            public virtual bool IsSuccessed
//            {
//                get
//                {
//                    return Exception == null && !Cancelled && IsCompleted;
//                }
//            }


//            public virtual bool Wait()
//            {
//                if (AsyncResult == null
//                    || AsyncResult.AsyncWaitHandle == null
//                    || Cancelled
//                    || IsCompleted)
//                {
//                    foreach (var item in _childs)
//                        if (!item.Wait())
//                            return false;
//                    return true;
//                }

//                bool flag = AsyncResult.AsyncWaitHandle.WaitOne();
//                if (flag && !IsCompleted)
//                {
//                    var mre = AsyncResult.AsyncWaitHandle as ManualResetEvent;
//                    if (mre != null 
//                        && !mre.SafeWaitHandle.IsClosed)
//                    {
//                        mre.Reset();
//                        if (mre.WaitOne())
//                        {
//                            foreach (var item in _childs)
//                                if (!item.Wait())
//                                    return false;
//                            return true;
//                        }

//                        return false;
//                    }
//                }
//                return flag;
//            }

//            public virtual bool Wait(int millisecondsTimeout)
//            {
//                if (AsyncResult == null
//                    || AsyncResult.AsyncWaitHandle == null
//                    || Cancelled
//                    || IsCompleted)
//                {
//                    foreach (var item in _childs)
//                        if (!item.Wait(millisecondsTimeout))
//                            return false;
//                    return true;
//                }

//                bool flag = AsyncResult.AsyncWaitHandle.WaitOne(millisecondsTimeout, false);
//                if (flag && !IsCompleted)
//                {
//                    var mre = AsyncResult.AsyncWaitHandle as ManualResetEvent;
//                    if (mre != null
//                        && !mre.SafeWaitHandle.IsClosed)
//                    {
//                        mre.Reset();
//                        if (mre.WaitOne())
//                        {
//                            foreach (var item in _childs)
//                                if (!item.Wait(millisecondsTimeout))
//                                    return false;
//                            return true;
//                        }

//                        return false;
//                    }
//                }
//                return flag;
//            }

//            private bool isCompleted = false;
//            public virtual bool IsCompleted
//            {
//                get
//                {
//                    if (AsyncResult == null)
//                        return true;
//                    if (Exception != null)
//                        return true;
//                    if (this.AsyncResult == null)
//                        return isCompleted;
//                    return AsyncResult.IsCompleted && isCompleted;
//                }
//                protected set
//                {
//                    if (isCompleted != value)
//                    {
//                        isCompleted = value;
//                    }
//                }
//            }

//            public virtual bool Cancelled { get; protected set; }

//            protected Tuple? cancelHandler;
//            protected struct Tuple
//            {
//                public CancelCallback Handler;
//                public object Arg;

//                public Tuple(CancelCallback handler, object arg)
//                {
//                    Handler = handler;
//                    Arg = arg;
//                }


//                public void Invoke()
//                {
//                    if (Handler != null)
//                        Handler.Send(Arg);
//                }
//            }

//            public virtual void Cancel()
//            {
//                Cancel(null, null);
//            }

//            public virtual void Cancel(CancelCallback callback, object arg)
//            {
//                this.cancelHandler = null;
//                if (callback != null)
//                    cancelHandler = new Tuple { Handler = callback, Arg = arg };

//                Cancelled = true;
//                IsCompleted = true;
//                if (AsyncResult == null || AsyncResult.AsyncWaitHandle == null)
//                {
//                    OnCompleted();
//                    return;
//                }

//                var mre = AsyncResult.AsyncWaitHandle as ManualResetEvent;
//                if (mre != null)
//                {
//                    if (cancelHandler.HasValue)
//                        cancelHandler.Value.Invoke();

//                    if (!mre.SafeWaitHandle.IsClosed)
//                    {
//                        mre.Set();
//                    }
//                }
//            }

//            public void Close()
//            {
//                if (AsyncResult != null
//                    && AsyncResult.AsyncWaitHandle != null
//                    && !AsyncResult.AsyncWaitHandle.SafeWaitHandle.IsClosed)
//                {
//                    try
//                    {
//                        AsyncResult.AsyncWaitHandle.Close();
//                    }
//                    catch (Exception ex)
//                    {
//                        ex.RaiseException();
//                    }
//                }
//            }

//            object IAsyncResult.AsyncState
//            {
//                get { return null; }
//            }

//            WaitHandle IAsyncResult.AsyncWaitHandle
//            {
//                get
//                {
//                    if (AsyncResult == null)
//                        return null;
//                    return AsyncResult.AsyncWaitHandle;
//                }
//            }

//            bool IAsyncResult.CompletedSynchronously
//            {
//                get
//                {
//                    if (AsyncResult == null)
//                        return true;
//                    return AsyncResult.CompletedSynchronously;
//                }
//            }
//        }

//        public class AsyncFuture<TResult> : SimpleFuture, IFuture<TResult>, IFutureContext<TResult>
//        {
//            public virtual TResult Value { get; protected internal set; }
//            public new Action<IFutureContext<TResult>> CompletedHandler { get; protected internal set; }
//        }

//    }


//}
