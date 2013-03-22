using System;
using System.Collections.Generic;
using System.Linq;
using NLite.Domain.Cfg;
using NLite.Domain.Mapping;
using NUnit.Framework;
using NLite.Cfg;
using NLite.Domain.Listener;


namespace NLite.Domain.Spec
{
    public class GivenAttribute : SetUpAttribute { }
    public class ThenAttribute : TestAttribute { }
    public class SpecificationAttribute : TestFixtureAttribute { }
    public class AndAttribute : ThenAttribute
    {
    }


    [Specification]
    public abstract class Specification<TSubjectUnderTest>:TestBase
    {
        protected TSubjectUnderTest SubjectUnderTest;
        protected Exception CaughtException;
        protected virtual void Given() { }
        protected virtual void When() { }
        protected virtual void Run() { }
        protected virtual void Close() { }

        protected override void OnInit()
        {
            try
            {
                Given();
                When();
                Run();

            }
            catch (Exception exception)
            {
                CaughtException = exception;
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }
            finally
            {
            }
        }
    }

    public abstract class ServiceRequestSpec : Specification<ServiceRequest>
    {
    }
   
  
   
    
  
}
