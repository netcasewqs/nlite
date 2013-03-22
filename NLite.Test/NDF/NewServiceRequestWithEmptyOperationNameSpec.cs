using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    public class NewServiceRequestWithEmptyOperationNameSpec : ServiceRequestSpec
    {
        protected override void Given()
        {
            base.Given();
            SubjectUnderTest = ServiceRequest.Create("Service1", "", null);
        }

        [Then]
        public void should_be_throw_ArgumentNullException()
        {
            Assert.IsNotNull(CaughtException);
            Assert.IsInstanceOf<ServiceDispatcherException>(CaughtException);
            Assert.AreEqual(ServiceDispatcherExceptionCode.OperationNameIsNullOrEmpty, (CaughtException as ServiceDispatcherException).ExceptionCode);
        }
    }
}
