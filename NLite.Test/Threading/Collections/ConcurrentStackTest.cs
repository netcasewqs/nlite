using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Threading.Collections;
using System.Threading;
using NLite.Collections;
using NLite.Threading;

namespace NLite.Test.Threading.Collections
{
    [TestFixture]
    public class ConcurrentStackTest
    {
        [Test]
        public void Test()
        {
            ConcurrentStack<int> stack = new ConcurrentStack<int>();
            stack.ForEach(i => Console.WriteLine(i));
            Console.WriteLine("---------------");
            CountDownLatch mutex = new CountDownLatch(20);
            for (int i = 1; i <= mutex.Count; i++)
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    stack.Push((int)state);
                    mutex.CountDown();
                }, i);
            }

            mutex.Await();
            stack.ForEach(i => Console.WriteLine(i));
           
        }
    }
}
