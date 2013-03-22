using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;
using System.Windows.Forms;

namespace NLite.Test
{
    //[TestFixture]
    public class SynchronizeServiceTest
    {
        ManualResetEvent mre;
        const string Title = "How are you!";
        Form mainForm;

        [Test]
        public void Test()
        {
            mre = new ManualResetEvent(false);
            mainForm = new Form();
           

            SynchronizeService.SynchronizeObject = mainForm;
            Action setTitle = () => SetTitle(mainForm);

            new Thread(() => RunMessageLoop(()=> SynchronizeService.Send(setTitle))) { Name = "UI", ApartmentState = ApartmentState.STA }.Start();
            mre.WaitOne();
            mre.Close();
        }

         [Test]
        public void PostTest()
        {
            mre = new ManualResetEvent(false);

            mainForm = new Form();
            SynchronizeService.SynchronizeObject = mainForm;
            Action setTitle = () => SetTitle(mainForm);
            new Thread(() => RunMessageLoop(() => SynchronizeService.Post(setTitle))) { Name = "UI", ApartmentState = ApartmentState.STA }.Start();

            mre.WaitOne();
            mre.Close();
        }

        void RunMessageLoop(Action fn)
        {
          
            //Action setTitle =()=> SetTitle(mainForm);

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;

            int counter =0;
            timer.Tick += (s, e) =>
            {
                counter++;
                if (counter == 3)
                {
                    timer.Stop();
                    timer.Dispose();

                    var title = mainForm.Text;
                    Assert.IsTrue(string.Equals(title, Title));

                    mainForm.Close();
                    mre.Set();
                }
            };

            mainForm.Load += (s, e) => ThreadPool.QueueUserWorkItem((state) => fn());
            mainForm.Shown += (s, e) => timer.Start();
            Application.Run(mainForm);
        }

        private void SetTitle(Form mainForm)
        {
            mainForm.Text = Title;
        }
    }
}
