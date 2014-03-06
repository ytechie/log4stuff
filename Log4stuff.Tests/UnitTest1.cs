using System.IO;
using System.Reflection;
using Log4stuff.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Log4stuff.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConvertLog4JToJson_SampleXml()
        {
            Stream embeddedStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(typeof (UnitTest1).Namespace + "." + "SampleLog4jXml.txt");

            string xml = new StreamReader(embeddedStream).ReadToEnd();
            string json = MvcApplication.ConvertLog4JToJson(xml);

            Assert.IsTrue(json.Length > 0);
        }

        [TestMethod]
        public void GetApplicationIdFromSampleXml()
        {
            Stream embeddedStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(typeof (UnitTest1).Namespace + "." + "SampleLog4jXml.txt");

            string xml = new StreamReader(embeddedStream).ReadToEnd();
            string applicationId = MvcApplication.GetApplicationId(xml);

            Assert.AreEqual("sample-app-id", applicationId);
        }
    }
}