using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Test.IoC.Addins.Components
{
    [Component]
    public class AddInManager:IAddInManager,IServiceReinjectedNotification
    {
        [InjectMany(Reinjection = true)]
        IEnumerable<Lazy<IAddIn, IAddInMetadata>> AddIns { get; set; }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                Console.WriteLine(string.Format("Name={0},Author={1},Url={2},Version={3}", item.Metadata.Name,item.Metadata.Author,item.Metadata.Url,item.Metadata.Version));
                item.Value.Start();
            }
        }

        public void Stop()
        {
        }

        public void OnReinjected()
        {
            Console.WriteLine(GetType().Name + " on reinjected.");
        }
    }


    [Component]
    public class ArrayMetadataAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        Lazy<IAddIn, IAddInMetadata>[] AddIns { get; set; }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                Console.WriteLine(string.Format("Name={0},Author={1},Url={2},Version={3}", item.Metadata.Name, item.Metadata.Author, item.Metadata.Url, item.Metadata.Version));
                item.Value.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class ArrayAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        IAddIn[] AddIns { get; set; }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                item.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class ArrayConstructureAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        private IAddIn[] AddIns;

        public ArrayConstructureAddInManager([InjectMany] IAddIn[] addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                item.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class ArrayLazy1ConstructureAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        private Lazy<IAddIn>[] AddIns;

        public ArrayLazy1ConstructureAddInManager([InjectMany] Lazy<IAddIn>[] addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                item.Value.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class ArrayLazy2ConstructureAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        private Lazy<IAddIn, IAddInMetadata>[] AddIns;

        public ArrayLazy2ConstructureAddInManager([InjectMany] Lazy<IAddIn, IAddInMetadata>[] addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                Console.WriteLine(string.Format("Name={0},Author={1},Url={2},Version={3}", item.Metadata.Name, item.Metadata.Author, item.Metadata.Url, item.Metadata.Version));
                item.Value.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class LazyEnumerableConstructureAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        private Lazy<IEnumerable<IAddIn>> AddIns;

        public LazyEnumerableConstructureAddInManager([InjectMany] Lazy<IEnumerable<IAddIn>> addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns.Value)
            {
                item.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class EnumerableConstructureAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        private IEnumerable<IAddIn> AddIns;

        public EnumerableConstructureAddInManager([InjectMany] IEnumerable<IAddIn> addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                item.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class EnumerableLazy1ConstructureAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        private IEnumerable<Lazy<IAddIn>> AddIns;

        public EnumerableLazy1ConstructureAddInManager([InjectMany] IEnumerable<Lazy<IAddIn>> addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                item.Value.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class EnumerableLazy2ConstructureAddInManager : IAddInManager
    {
        [InjectMany(Reinjection = true)]
        private IEnumerable<Lazy<IAddIn, IAddInMetadata>> AddIns;

        public EnumerableLazy2ConstructureAddInManager([InjectMany] IEnumerable<Lazy<IAddIn, IAddInMetadata>> addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                Console.WriteLine(string.Format("Name={0},Author={1},Url={2},Version={3}", item.Metadata.Name, item.Metadata.Author, item.Metadata.Url, item.Metadata.Version));
                item.Value.Start();
            }
        }

        public void Stop()
        {
        }
    }

    [Component]
    public class ArrayMethodAddInManager : IAddInManager
    {
        [IgnoreAttribute]
        private IAddIn[] AddIns;

        [Inject]
        public void SetAddIns([InjectMany(Reinjection=true)]IAddIn[] addins)
        {
            AddIns = addins;
        }

        public void Start()
        {
            foreach (var item in AddIns)
            {
                item.Start();
            }
        }

        public void Stop()
        {
        }
    }
}
