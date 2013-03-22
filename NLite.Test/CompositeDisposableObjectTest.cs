using System;
using NUnit.Framework;

namespace NLite.Test
{
    [TestFixture]
    public class CompositeDisposableObjectTest
    {
        class DisposeObject1 : BooleanDisposable
        {
            protected override void Dispose(bool disposing)
            {
                Console.WriteLine("Dispose 1");
            }
        }

        class DisposeObject2 : BooleanDisposable
        {
            protected override void Dispose(bool disposing)
            {
                Console.WriteLine("Dispose 2");
            }
        }


        [Test]
        public void Test()
        {
            var disposes = new CompositeDisposable();
            var dis1 = new DisposeObject1();
            var dis2 = new DisposeObject2();

            Assert.IsFalse(disposes.IsDisposed);
            Assert.IsFalse(dis1.IsDisposed);
            Assert.IsFalse(dis2.IsDisposed);

            disposes.AddDisposable(dis1);
            disposes.AddDisposable(dis2);

            disposes.Dispose();
            Assert.IsTrue(disposes.IsDisposed);
            Assert.IsTrue(dis1.IsDisposed);
            Assert.IsTrue(dis2.IsDisposed);

          


        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void DisposeTest()
        {
            var disposes = new CompositeDisposable();
            var dis1 = new DisposeObject1();
           
            disposes.Dispose();
         

            disposes.AddDisposable(new DisposeObject1());
        }

    }
}
