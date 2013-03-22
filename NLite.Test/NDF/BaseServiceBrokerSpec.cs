using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Domain;

namespace NLite.Domain.Spec
{
    public abstract class BaseServiceBrokerSpec : Specification<IServiceDispatcherConfiguationItem>
    {
        protected override void Given()
        {
            ServiceRegistry
                .Compose(this);
        }
    }
}
