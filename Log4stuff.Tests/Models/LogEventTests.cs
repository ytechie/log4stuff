using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Log4stuff.Web.Models
{
    [TestClass]
    public class LogEventTests
    {
        [TestMethod]
        public void ToJsonAndBack()
        {
            var le = new LogEvent
            {
                Level = "dbg",
                Logger = "lgr",
                Message = "msg",
                Thread = "thd",
                Timestamp = DateTime.Parse("2014-06-26 10:48am"),
                ApplicationId = "apl"
            };
            le.Metadata.Add("a", "b");
            le.Metadata.Add("c", "d");
            var json = le.ToJson();

            var le2 = new LogEvent();
            le2.PopulateFromJson(json);

            Assert.AreEqual("dbg", le2.Level);
            Assert.AreEqual("lgr", le2.Logger);
            Assert.AreEqual("msg", le2.Message);
            Assert.AreEqual("thd", le2.Thread);
            Assert.AreEqual(DateTime.Parse("2014-06-26 10:48am"), le2.Timestamp);
            Assert.AreEqual("b", le2.Metadata["a"]);
            Assert.AreEqual("d", le2.Metadata["c"]);
            Assert.AreEqual("apl", le2.ApplicationId);
        }

        [TestMethod]
        public void ParseLog4JXml()
        {
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(LogEventTests), "SampleLog4JXml.xml");
            var xml = new StreamReader(stream).ReadToEnd();

            var le = new LogEvent();
            le.PopulateFromLog4JXml(xml);

            Assert.AreEqual("Simulator", le.Logger);
            Assert.AreEqual(DateTime.Parse("2014-06-26 3:33:54.8050000 PM"), le.Timestamp);
            Assert.AreEqual("DEBUG", le.Level);
            Assert.AreEqual("7", le.Thread);
            Assert.AreEqual("log4stuff", le.ApplicationId);

            Assert.AreEqual("speedyjr", le.Metadata["log4net:HostName"]);
            Assert.AreEqual("speedyjr", le.Metadata["log4jmachinename"]);
            Assert.AreEqual("", le.Metadata["log4net:Identity"]);
            Assert.AreEqual("SPEEDYJR\\Jason", le.Metadata["log4net:UserName"]);
            Assert.AreEqual("/LM/W3SVC/10/ROOT-1-130482703312877309", le.Metadata["log4japp"]);
        }
    }
}
