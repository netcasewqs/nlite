using System.Reflection;
using NLite.Globalization;
using NUnit.Framework;

namespace NLite.Test
{
    [TestFixture]
    public class ResourceRespositoryTest
    {
        string initLanguage;
        [SetUp]
        public void Init()
        {
            initLanguage = LanguageManager.Instance.Language;
        }

        [TearDown]
        public void Release()
        {
            LanguageManager.Instance.Language = initLanguage;
        }

        [Test]
        public void Test()
        {
            var strResourceMgr = ResourceRepository.StringRegistry;
            Assert.IsNotNull(strResourceMgr);

            LanguageManager.Instance.Language = "en";

            strResourceMgr.Register("NLite.Test.App_LocalResources", Assembly.GetExecutingAssembly());

            var elementMapValue = ResourceRepository.Strings.Get("ElementMap");
            Assert.IsNotNull(elementMapValue);
            Assert.IsTrue(string.Equals(elementMapValue, "ElementMap"));

            LanguageManager.Instance.Language = "zh-cn";

            elementMapValue = ResourceRepository.Strings.Get("ElementMap");
            Assert.IsNotNull(elementMapValue);
            Assert.IsTrue(string.Equals(elementMapValue, "UI 元素映射测试"));



        }
    }
}
