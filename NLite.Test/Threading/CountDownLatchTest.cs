using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Threading;
using System.Threading;

namespace NLite.Test.Threading
{
    [TestFixture]
    public class CountDownLatchTest
    {
        [Test]
        public void Test()
        {
            Populate(1);
            Populate(2);
            Populate(10);
        }

        private static void Populate(int n)
        {
            var mutex = new CountDownLatch(n);
            for (int i = 1; i <= mutex.Count; i++)
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    Console.WriteLine(state);
                    mutex.CountDown();
                }, i);
            }

            mutex.Await();
        }
    }
}
