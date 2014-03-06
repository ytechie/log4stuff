using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
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
                    .GetManifestResourceStream(typeof(UnitTest1).Namespace + "." + "SampleLog4jXml.txt");

            string xml = new StreamReader(embeddedStream).ReadToEnd();
            string json = MvcApplication.ConvertLog4JToJson(xml);

            Assert.IsTrue(json.Length > 0);
        }

        [TestMethod]
        public void GetApplicationIdFromSampleXml()
        {
            Stream embeddedStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(typeof(UnitTest1).Namespace + "." + "SampleLog4jXml.txt");

            string xml = new StreamReader(embeddedStream).ReadToEnd();
            string applicationId = MvcApplication.GetApplicationId(xml);

            Assert.AreEqual("sample-app-id", applicationId);
        }

        [TestMethod]
        public void MessageFlood()
        {
            var udpAppender = new UdpAppender
            {
                RemoteAddress = IPAddress.Parse("137.135.86.175"),
                RemotePort = 8080,
                Name = "UDPAppender",
                Encoding = new ASCIIEncoding(),
                Threshold = Level.Debug,
                Layout = new XmlLayoutSchemaLog4j()
            };
            udpAppender.ActivateOptions();

            BasicConfigurator.Configure(udpAppender);

            var log = LogManager.GetLogger("Simulator");
            log4net.GlobalContext.Properties["ApplicationId"] = "902245b6-aec4-4057-bd7f-b78264cf9455";

            for (var i = 0; i < 10000; i++)
            {
                log.DebugFormat("Flood #{0}", i);
            }
        }
    }
}