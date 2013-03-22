using System;
using System.Threading;
using NLite.Threading;
using NUnit.Framework;

namespace NLite.Test.Threading
{
    [TestFixture]
    public class ActiveObjectTest
    {
        [Test]
        public void Test()
        {

            var activeObject = new ActiveObject();
            for (int i = 1; i < 11; i++)
            {
                var m = i;
                activeObject.AddCommand(() =>
                {
                    Console.WriteLine(m);
                    Console.WriteLine(System.Threading.Thread.CurrentThread.IsBackground);
                    System.Threading.Thread.Sleep(500);

                    Console.WriteLine();
                });
            }

            activeObject.Start();


            System.Threading.Thread.Sleep(4000);
            activeObject.Stop();
        }

        [Test]
        public void Test2()
        {
            using (var activeObject = new ActiveObject())
            {

                activeObject.Start();
                for (int i = 1; i < 11; i++)
                {
                    var m = i;
                    activeObject.AddCommand(() =>
                    {
                        Console.WriteLine(m);
                        Console.WriteLine(Thread.CurrentThread.IsBackground);
                        System.Threading.Thread.Sleep(500);

                        Console.WriteLine();
                    });
                }

                Thread.Sleep(4000);
            }
        }

        [Test]
        public void Test3()
        {
            using (var activeObject = new ActiveObject())
            {

                activeObject.Start();
                for (int i = 1; i < 11; i++)
                {
                    var m = i;
                    activeObject.AddCommand(() =>
                    {
                        Console.WriteLine(m);
                        Console.WriteLine(Thread.CurrentThread.IsBackground);
                        Thread.Sleep(500);

                        Console.WriteLine();
                    });
                }

                Thread.Sleep(4000);
            }
        }

        [Test]
        public void Test4()
        {
            for (int i = 0; i < 5; i++)
                DoTest();
        }

        private static void DoTest()
        {
            Thread.Sleep(50);
            using (var activeObject = new ActiveObject())
            {

                activeObject.Start();
                activeObject.Interval = 500;

                for (int i = 1; i < 11; i++)
                {
                    var m = i;
                    activeObject.AddCommand(() =>
                    {
                        Console.WriteLine(m);
                        Console.WriteLine(Thread.CurrentThread.IsBackground);
                        Thread.Sleep(500);

                        Console.WriteLine();
                    });
                }

                Thread.Sleep(8000);
            }
        }
    }
}

