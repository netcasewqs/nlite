//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using NLite.ComponentModel;
//using System.Windows.Forms;
//using NLite.Globalization;
//using System.Reflection;
//using System.Threading;

//namespace NLite.Test.ComponentModel
//{
//    [TestFixture]
//    public class ComponentObjectTest
//    {
//        public enum Role
//        {
//            [EnumDescription(ResourceKey= "Role.Administrator")]
//            Administrator,

//            [EnumDescription(ResourceKey= "Role.Administrator")]
//            Normal,
//        }

//        public class Employee : ComponentObject
//        {
//            string name;
//            [LocalizedProperty(Name = "${res:Employee.Name}", Category = "${res:Employee.Category}", Description = "${res:Employee.Name.Description}")]
//            public string Name
//            {
//                get { return name; }
//                set
//                {
//                    name = PopulateValue<string>("Name", value, name);
//                }
//            }

//            public Role Role { get; set; }
//        }

//        [Test]
//        public void Test()
//        {
//            var p = new Employee();
//            p.PropertyChanged += (s, e) =>
//            {
//                var e2 = e as PropertyChangedEventArgs;
//                Console.WriteLine(string.Format("Property :{0}, Old:{1}, New:{2}", e2.PropertyName,e2.OldValue,e2.NewValue));
//            };
//            Assert.IsFalse(p.IsChanged);
//            Assert.IsFalse(p.IsEdit);

//            p.BeginEdit();
//            Assert.IsFalse(p.IsChanged);
//            Assert.IsTrue(p.IsEdit);
//            p.Name = "sf";
//            Assert.IsTrue(p.IsChanged);

//            p.Name = p.Name;


//            p.EndEdit();
//            Assert.IsTrue(p.IsChanged);
//            Assert.IsFalse(p.IsEdit);

//            p.AcceptChanges();
//            Assert.IsFalse(p.IsChanged);
//            Assert.AreEqual("sf", p.Name);

//            p.BeginEdit();
//            p.Name = "sfs";
//            Assert.AreEqual("sfs", p.Name);
//            p.CancelEdit();
//            Assert.AreEqual("sf", p.Name);

//            p.Name = "sf2";
//            Assert.AreEqual("sf2", p.Name);
//            p.CancelEdit();
//            Assert.AreEqual("sf2", p.Name);
//            p.RejectChanges();
//            Assert.AreEqual("sf", p.Name);


//            p.Name = "sf2";
//            Assert.AreEqual("sf2", p.Name);
//            p.CancelEdit();
//            Assert.AreEqual("sf2", p.Name);
//            p.RejectChanges();
//            Assert.AreEqual("sf", p.Name);


//            p.Name = "sf2";
//            Assert.AreEqual("sf2", p.Name);
//            p.CancelEdit();
//            Assert.AreEqual("sf2", p.Name);
//            p.AcceptChanges();
//            Assert.AreEqual("sf2", p.Name);
//            p.RejectChanges();
//            Assert.AreEqual("sf2", p.Name);
//        }

//        [Test]
//        public void CustomTypeDescriptorTest()
//        {
//            var desc = new LocalTypeDescriptionProvider().GetTypeDescriptor(new Employee());
            
//            Assert.IsNotNull(desc);

//            Assert.AreEqual("NLite.Test.ComponentModel.ComponentObjectTest+Employee",desc.GetClassName());

//            Console.WriteLine(desc.GetComponentName());

//            Assert.IsInstanceOf<System.ComponentModel.TypeConverter>(desc.GetConverter());

//            Assert.AreEqual("PropertyChanged", desc.GetDefaultEvent().Name);

//            var attrs = desc.GetAttributes();

//            Console.WriteLine(attrs.Count);

//            foreach (var attr in attrs)
//                Console.WriteLine(attr.ToString());

//            var ps = desc.GetProperties();
//            Console.WriteLine(ps.Count);

//            foreach (var item in ps)
//            {
//                Assert.IsInstanceOf<LocalizedPropertyDescriptor>(item);
//            }
//        }


//        ManualResetEvent mre;
//        const string Title = "How are you!";
//        Form mainForm;
//        PropertyGrid grid = new PropertyGrid();

//        string initLanguage;
//        [SetUp]
//        public void Init()
//        {
//            initLanguage = "en";
//            LanguageManager.Instance.Language = initLanguage;
//        }

//        [TearDown]
//        public void Release()
//        {
//            LanguageManager.Instance.Language = initLanguage;
//        }


//        [Test]
//        public void LocalTest()
//        {
//            //var strResourceMgr = ResourceRepository.StringRegistry;
//            //Assert.IsNotNull(strResourceMgr);

//            //strResourceMgr.Register("NLite.Test.Data.Resources", Assembly.GetExecutingAssembly());


//            //mre = new ManualResetEvent(false);
//            //mainForm = new Form();

//            //mainForm.Controls.Add(grid);
//            //grid.Dock = DockStyle.Fill;
//            //grid.SelectedObject = new Employee { Name = "Zhang San" };

//            //new Thread(() => RunMessageLoop()) { Name = "UI", ApartmentState = ApartmentState.STA }.Start();


//            //mre.WaitOne(16 * 1000);
//            //mre.Close();
//        }


//        void RunMessageLoop()
//        {

//            //Action setTitle =()=> SetTitle(mainForm);

//            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
//            timer.Interval = 1000;

//            int counter = 0;
//            timer.Tick += (s, e) =>
//            {
//                counter++;
//                if (counter == 10)
//                {
//                    LanguageManager.Instance.Language = "zh-cn";
//                    grid.SelectedObject = grid.SelectedObject;
//                    grid.Invalidate();
//                }
//                else if (counter == 15)
//                {
//                    timer.Stop();
//                    timer.Dispose();


//                    mainForm.Close();
//                    mre.Set();
//                }
//            };


//            mainForm.Shown += (s, e) => timer.Start();

//            Application.Run(mainForm);
//        }

//    }
//}
