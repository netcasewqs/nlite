//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using NLite.Threading;
//using System.Threading;

//namespace NLite.Test
//{
//    [TestFixture]
//    public class FutureTest
//    {
//        [Test]
//        public void SimpleTest()
//        {
//            var mre = new ManualResetEvent(false);
//            Future.Create(() =>
//            {
//                mre.Set();
//                Console.WriteLine("Future:" + Thread.CurrentThread.ManagedThreadId);
//                Console.WriteLine("FutureId:" + Future.Current.Id);
//            });

//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
//            mre.WaitOne();
//        }

//        [Test]
//        public void SimpleTest2()
//        {
//            IFuture future = null;
//            future = Future.Create(() =>
//             {
//                 Assert.IsFalse(future.IsSuccessed);
//                 Assert.IsFalse(future.IsCompleted);

//                 future.Set();
//                 Console.WriteLine("Future:" + Thread.CurrentThread.ManagedThreadId);
//                 Console.WriteLine("FutureId:" + future.Id);
//             });

//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
//            future.Wait();

//            Assert.IsTrue(future.IsSuccessed);
//            Assert.IsTrue(future.IsCompleted);
//        }

//        [Test]
//        public void ExceptionTest()
//        {
//            IFuture future = null;
//            future = Future.Create(() =>
//            {
//                Assert.IsFalse(future.IsSuccessed);
//                Assert.IsFalse(future.IsCompleted);

//                Console.WriteLine("FutureId:" + future.Id);
//                throw new Exception();
//            });

//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
//            future.Wait();

//            Assert.IsNotNull(future.Exception);
//            Assert.IsFalse(future.IsSuccessed);
//            Assert.IsTrue(future.IsCompleted);
//        }

//        [Test]
//        public void CancelTest()
//        {
//            IFuture future = null;
//            future = Future.Create(() =>
//            {
//                Thread.SpinWait(4000);
//                Assert.IsFalse(future.IsSuccessed);
//                Assert.IsFalse(future.IsCompleted);
//                Assert.IsFalse(future.Cancelled);

//                Console.WriteLine("FutureId:" + future.Id);
//                future.Cancel();
//            });

//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
//            future.Wait();

//            Assert.IsFalse(future.IsSuccessed);
//            Assert.IsTrue(future.IsCompleted);
//            Assert.IsTrue(future.Cancelled);
//        }


//        [Test]
//        public void CancelTest2()
//        {
//            IFuture future = null;
//            future = Future.Create(() =>
//            {
//                Assert.IsFalse(future.IsSuccessed);
//                Assert.IsFalse(future.IsCompleted);
//                Assert.IsFalse(future.Cancelled);

//                Console.WriteLine("FutureId:" + future.Id);
//                future.Cancel((arg) => Console.WriteLine("cancel future " + future.Id), null);
//            });

//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
//            future.Wait();

//            Assert.IsFalse(future.IsSuccessed);
//            Assert.IsTrue(future.IsCompleted);
//            Assert.IsTrue(future.Cancelled);
//        }

//        [Test]
//        public void CloseTest()
//        {
//            IFuture future = null;
//            future = Future.Create(() =>
//            {
//                Assert.IsFalse(future.IsSuccessed);
//                Assert.IsFalse(future.IsCompleted);

//                Console.WriteLine("Future:" + Thread.CurrentThread.ManagedThreadId);
//                Console.WriteLine("FutureId:" + future.Id);
//            });

//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
//            future.Wait();

//            Assert.IsTrue(future.IsSuccessed);
//            Assert.IsTrue(future.IsCompleted);
//            future.Close();
//        }

//        [Test]
//        public void CloseTest2()
//        {
//            IFuture future = null;
//            future = Future.Create(() =>
//            {
//                Assert.IsFalse(future.IsSuccessed);
//                Assert.IsFalse(future.IsCompleted);


//                Console.WriteLine("Future:" + Thread.CurrentThread.ManagedThreadId);
//                Console.WriteLine("FutureId:" + future.Id);
//            });

//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
//            future.Wait();

//            Assert.IsTrue(future.IsSuccessed);
//            Assert.IsTrue(future.IsCompleted);
//            future.Close();

//            future = Future.Create(() => Console.WriteLine("FutureId:" + future.Id));
//            future.Wait(5);
//            future.Close();
//        }


//        [Test]
//        public void ReturnValueTest()
//        {
//            IFuture<int> future = null;
//            future = Future.Create<int>(() =>
//            {
//                Assert.IsFalse(future.IsSuccessed);
//                Assert.IsFalse(future.IsCompleted);

//                Console.WriteLine("Future:" + Thread.CurrentThread.ManagedThreadId);
//                Console.WriteLine("FutureId:" + future.Id);

//                Assert.IsNull(future.AsyncState);
//                Assert.AreSame(future.AsyncResult.AsyncWaitHandle, future.AsyncWaitHandle);

//                return 100;
//            });

//            future.Wait();
//            Assert.IsTrue(future.Value == 100);
//        }

//        [Test]
//        public void WaitAllTest()
//        {
//            int counter = 0;
//            for (int i = 0; i < 10; i++)
//                Future.Create(() => counter++);

//            Future.WaitAll();
//            Console.WriteLine(counter);
//            Assert.IsTrue(counter == 10);
//        }

//        [Test]
//        public void WaitAllTest2()
//        {
//            var futures = new List<IFuture>();
//            for (int i = 0; i < 10; i++)
//                futures.Add(Future.Create(() => Console.WriteLine(Future.Current.Id)));

//            Future.WaitAll(futures);

//            foreach (var item in futures)
//                Assert.IsTrue(item.IsCompleted && item.IsSuccessed);
//        }

//        [Test]
//        public void NestFutureTest()
//        {
//            IFuture future = null;
//            IFuture future2 = null;
//            IFuture future3 = null;

//            future = Future.Create(() =>
//                {
//                    Console.WriteLine(Future.Current.Id);


//                    future2 = Future.Create(() =>
//                        {
//                            Console.WriteLine(Future.Current.Id);

//                            Assert.IsTrue(future2.Parent == future);

//                            future3 = Future.Create(() =>
//                            {
//                                Console.WriteLine(Future.Current.Id);
//                                Assert.IsTrue(future3.Parent == future2);
//                                future.Set();
//                            });

//                            //future3.Wait();
//                        });

//                    //future2.Wait();
//                    //future.Set();
//                });

//            Assert.IsNull(future.Parent);
//            future.Wait();

//            Assert.IsTrue(future.IsCompleted);
//            Assert.IsTrue(future2.IsCompleted);
//            Assert.IsTrue(future3.IsCompleted);
//        }
//    }
//}
