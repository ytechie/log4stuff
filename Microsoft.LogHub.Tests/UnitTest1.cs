using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.LogHub
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConvertLog4JToJson_SampleXml()
        {
            var embeddedStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(typeof(UnitTest1).Namespace + "." + "SampleLog4jXml.txt");

            var xml = new StreamReader(embeddedStream).ReadToEnd();
            var json = MvcApplication.ConvertLog4JToJson(xml);

            Assert.IsTrue(json.Length > 0);
        }
    }
}
