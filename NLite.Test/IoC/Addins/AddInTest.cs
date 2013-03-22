using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite;
using NLite.Test.IoC.Addins.Components;
using NUnit.Framework;
using NLite.Mini.Resolving;

namespace NLite.Test.IoC.Addins
{
    [TestFixture]
    public class AddInTest:TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ReferenceManager.Instance.Enabled = true;
            ServiceRegistry
                .Register<AddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
            ReferenceManager.Instance.Enabled = false;
        }
    }

    [TestFixture]
    public class ArrayAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<ArrayAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
//            ServiceRegistry.Register<PlaylistAddIn>();
//            AddInManager.Start();
        }
    }


    [TestFixture]
    public class ArrayMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<ArrayMetadataAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class ArrayConstructureMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<ArrayConstructureAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class ArrayLazy1ConstructureMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<ArrayLazy1ConstructureAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class ArrayLazy2ConstructureMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<ArrayLazy2ConstructureAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class LazyEnumerableConstructureMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<LazyEnumerableConstructureAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class EnumerableConstructureMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<EnumerableConstructureAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class EnumerableLazy1ConstructureMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<EnumerableLazy1ConstructureAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class EnumerableLazy2ConstructureMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<EnumerableLazy2ConstructureAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }

    [TestFixture]
    public class ArrayMethodMetadataAddInTest : TestBase
    {
        [Inject]
        IAddInManager AddInManager;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<ArrayMethodAddInManager>()
                .Register<DashboardAddIn>()
                .Register<EditorAddIn>()
                ;


            ServiceRegistry.Compose(this);
            ServiceRegistry.Register<PlaylistAddIn>();
            AddInManager.Start();
        }
    }
}
