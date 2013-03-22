using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Domain.Spec;
using NLite.Domain;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    public class NewServiceRequestWithNoArgsSpec : ServiceRequestSpec
    {
        protected override void Given()
        {
            base.Given();
            SubjectUnderTest = ServiceRequest.Create(null, null, new Dictionary<string, object>());
        }

        [Then]
        public void should_be_throw_ArgumentNullException()
        {
            Assert.IsNotNull(CaughtException);
            Assert.IsInstanceOf<ServiceDispatcherException>(CaughtException);
            Assert.AreEqual(ServiceDispatcherExceptionCode.ServiceNameIsNullOrEmpty, (CaughtException as ServiceDispatcherException).ExceptionCode);
        }
    }

}
