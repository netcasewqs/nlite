using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NLite.Test.IoC.Contracts.Components
{
    [Metadata("TestClassName","SimpleComponent")]
    public class SimpleComponent : ISimpleContract, INonContractInterface, IDisposable, IInitializable, ISupportInitialize
    {
        public void Dispose() { }

        public void Init()
        {
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }
    }
}
