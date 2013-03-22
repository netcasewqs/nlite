using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Test.IoC.Contracts.Components;

namespace NLite.Test.IoC.Contracts
{
    [TestFixture]
    public class ContractServiceTest
    {
        [Test]
        public void Test()
        {
            var results = ContractService.GetContracts(typeof(ISimpleContract));
            Assert.IsTrue(results.Count() == 1);

            results = ContractService.GetContracts(typeof(SimpleComponent));
            Assert.IsTrue(results.Count() == 3);
        }
    }
}
