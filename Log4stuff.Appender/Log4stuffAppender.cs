using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace Log4stuff.Appender
{
    public class Log4stuffAppender : UdpAppender
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string ApplicationId { private get; set; }

        public override void ActivateOptions()
        {
            var ip = Dns.GetHostEntry("log4stuff.com");
            RemoteAddress = ip.AddressList[0];
            RemotePort = 8080;
            base.Layout = new XmlLayoutSchemaLog4j();

            log4net.GlobalContext.Properties["ApplicationId"] = ApplicationId;

            var logUrl = string.Format("http://log4stuff.com/app/{0}", ApplicationId);
            Console.WriteLine(string.Format("Now logging to {0}", logUrl));
            Log.DebugFormat("Now logging to {0}", logUrl);

            base.ActivateOptions();
        }

        public static void AutoConfigureLogging(string applicationId, bool listenToTrace = true)
        {
            var udpAppender = new UdpAppender
            {
                RemoteAddress = Dns.GetHostAddresses("log4stuff.com").First(),
                RemotePort = 8080,
                Layout = new XmlLayoutSchemaLog4j()
            };
            udpAppender.ActivateOptions();
            BasicConfigurator.Configure(udpAppender);
            log4net.GlobalContext.Properties["ApplicationId"] = applicationId;

            if (listenToTrace)
            {
                Trace.Listeners.Add(new Log4NetTraceListener());
            }
        }
    }
}
