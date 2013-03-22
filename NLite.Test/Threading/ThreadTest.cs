using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace NLite.Test.Threading
{
    [TestFixture]
    public class ThreadTest
    {
        
        [Test]
        public void SimpleTest()
        {
            var goodThread = new  Thread(() =>
                {
                    Console.WriteLine("Good");

                    Assert.IsFalse(Thread.CurrentThread.IsThreadPoolThread);
                    Assert.IsFalse(Thread.CurrentThread.IsBackground);

                    Assert.IsTrue(Thread.CurrentThread.IsAlive);
                    Assert.IsTrue(Thread.CurrentThread.ApartmentState == ApartmentState.MTA);

                    Assert.IsTrue(Thread.CurrentThread.Priority == ThreadPriority.Normal);
                    Assert.IsTrue(Thread.CurrentThread.ThreadState == ThreadState.Running);

                    ThreadInterruptedException Exception = null;
                    try
                    {
                        Thread.Sleep(5000);
                    }
                    catch (ThreadInterruptedException ex)
                    {
                        Exception = ex;
                    }

                    Assert.IsNotNull(Exception);
                });

            goodThread.Start();
            Thread.Sleep(50);

            goodThread.Interrupt();
        }


    }
}
