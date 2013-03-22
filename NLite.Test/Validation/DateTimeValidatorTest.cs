using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using NLite.Validation;

namespace NLite.Test.Validation
{
    [TestFixture]
    public class DateTimeValidatorTest
    {
        /// <summary>
        /// 编译时校验绑定
        /// </summary>
        public class TestTarget
        {

            [Required]
            public DateTime? DtField { get; set; }
        }

        /// <summary>
        /// 松耦合的编译时校验绑定
        /// </summary>
        [MetadataType(typeof(TestTarget2Metadata))]
        public class TestTarget2
        {

            public DateTime? DtField { get; set; }
        }
        
        //更松耦合的运行时关联校验绑定
        [EntityValidator("NLite.Test.Validation.DateTimeValidatorTest+TestTarget2Metadata,NLite.Test")]
        public class TestTarget3
        {

            public DateTime? DtField { get; set; }
        }

        public class TestTarget4
        {
            public DateTime? DtField { get; set; }
        }

        public class TestTarget2Metadata
        {
            [Required]
            public DateTime? DtField { get; set; }
        }

        [Test]
        public void Test()
        {

            //编译时校验绑定
            var t = new TestTarget();
            var rs = NLite.Validation.Validator.Validate<TestTarget>(t);
            Assert.IsFalse(rs.IsValid);

            //松耦合的编译时校验绑定（通过MetadataTypeAttribute）
            var t2 = new TestTarget2();
            var rs2 = NLite.Validation.Validator.Validate<TestTarget2>(t2);
            Assert.IsFalse(rs2.IsValid);

            //更松耦合的运行时关联校验绑定（通过EntityValidator）
            Console.WriteLine(typeof(TestTarget2Metadata).FullName);
            var t3 = new TestTarget3();
            var rs3 = NLite.Validation.Validator.Validate<TestTarget3>(t3);
            Assert.IsFalse(rs3.IsValid);

            //最灵活的运行时关联校验绑定（通过手工注册绑定）
            Validator.Register<TestTarget4,TestTarget2Metadata>();
            var t4 = new TestTarget4();
            var rs4 = NLite.Validation.Validator.Validate<TestTarget4>(t4);
            Assert.IsFalse(rs4.IsValid);
        }
    }


}
