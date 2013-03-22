using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;
using NLite.Threading;
using System.Net;

namespace NLite.Test.Threading
{
    [TestFixture]
    public class EventWaitHandleTest
    {
        [SetUp]
        public void SetUp()
        {
            CodeTimer.Initialize();
        }

        [Test]
        public void Test()
        {
            CodeTimer.Time("ManualRestEvent(false)", () =>//无信号,可以通过WaitOne 阻塞线程的执行,通过Set发出信号唤醒等待的线程
            {
                using (System.Threading.ManualResetEvent are = new System.Threading.ManualResetEvent(false))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((s) =>
                        {
                            Thread.Sleep(2000);
                            Console.WriteLine("Run!");
                            are.Set();
                        });
                    Console.WriteLine(are.WaitOne(5000));
                }

            });

            CodeTimer.Time("ManualRestEvent(true)", () =>//有信号表示终止状态，即线程属于闲置状态
            {
                using (System.Threading.ManualResetEvent are = new System.Threading.ManualResetEvent(true))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((s) =>
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine("Not Run!");
                        if(!are.SafeWaitHandle.IsClosed)
                            are.Set();
                    });
                    are.WaitOne();//不会等待子线程的结束
                }

            });

            CodeTimer.Time("ManualRestEvent(reset)", () =>
            {
                using (System.Threading.ManualResetEvent are = new System.Threading.ManualResetEvent(true))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((s) =>
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine("Run!");
                        are.Set();
                    });
                    are.WaitOne();//不会等待子线程的结束
                    are.Reset();//
                    are.WaitOne();
                }

            });
        }

        [Test]
        public void AutoRestEventTest()
        {
            CodeTimer.Time("AutoResetEvent(false)", () =>//无信号,可以通过WaitOne 阻塞线程的执行,通过Set发出信号唤醒等待的线程
            {
                using (System.Threading.AutoResetEvent are = new System.Threading.AutoResetEvent(false))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((s) =>
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine("Run!");
                        are.Set();
                    });
                    are.WaitOne();
                }

            });

            CodeTimer.Time("AutoResetEvent(true)", () =>//有信号表示终止状态，即线程属于闲置状态
            {
                using (System.Threading.AutoResetEvent are = new System.Threading.AutoResetEvent(true))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((s) =>
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine("Not Run!");
                        are.Set();
                    });
                    are.WaitOne();//不会等待子线程的结束
                }

            });

        }

        [Test]
        public void ParallerTest()
        {
            int loopCount = 1;
            int arrayLength = 10 * 1000 * 1000;

            Console.WriteLine(Environment.ProcessorCount);

            int maxWorkThreads = 0;
            int maxCompletionPortThreads = 0;
            ThreadPool.GetMaxThreads(out maxWorkThreads, out maxCompletionPortThreads);
            Console.WriteLine(maxWorkThreads);
            Console.WriteLine(maxCompletionPortThreads);


            int minWorkThreads = 0;
            int minCompletionPortThreads = 0;
            ThreadPool.GetMinThreads(out minWorkThreads, out minCompletionPortThreads);
            Console.WriteLine(minWorkThreads);
            Console.WriteLine(minCompletionPortThreads);

            int workThreads = 0;
            int completionPortThreads = 0;
            ThreadPool.GetAvailableThreads(out workThreads, out completionPortThreads);
            Console.WriteLine(workThreads);
            Console.WriteLine(completionPortThreads);



            CodeTimer.Time("Paraller.For", loopCount, () =>
            {
                Paraller.For(0, arrayLength, i =>
                {
                    var m = i + 1;
                });

            });

            CodeTimer.Time("For ", loopCount, () =>
            {
                var array = Enumerable.Range(0, arrayLength);
                foreach (var i in array)
                {
                    var m = i + 1;
                }

            });


            var result = new List<int>(10);
            CodeTimer.Time("Sum ", 1, () =>
            {
                Paraller.For(0, 10, i =>
                {
                    Thread.Sleep(i * 10);
                    lock (this)
                        result.Add(i);
                });
            });

            Thread.Sleep(2000);

            result.ForEach(i => Console.WriteLine(i));
            Console.WriteLine(result.Sum());

            var urls = new string[] { 
               
            };


            CodeTimer.Time("Paraller.Urls", loopCount, () =>
            {

                Paraller.For(0, urls.Length, i =>
                {
                    new WebClient().OpenRead(urls[i]);
                    //lock(typeof(Console))
                    Console.WriteLine(urls[i]);
                });

            });

            CodeTimer.Time("urls ", loopCount, () =>
            {
                foreach (var url in urls)
                {
                    new WebClient().OpenRead(url);
                    Console.WriteLine(url);
                }

            });


        }


    }
}
