using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace Log4stuff.Sample
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main()
        {
            ConfigureWithCode();

            var timer = new Timer((state) => Log.Debug("Sample Message"), null, 1000, 1000);

            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

        private static void ConfigureWithCode()
        {
            var udpAppender = new UdpAppender
            {
                RemoteAddress = Dns.GetHostAddresses("log4stuff.com").First(),
                RemotePort = 8080,
                Layout = new XmlLayoutSchemaLog4j()
            };
            udpAppender.ActivateOptions();

            BasicConfigurator.Configure(udpAppender);

            var applicationId = Guid.NewGuid();
            log4net.GlobalContext.Properties["ApplicationId"] = applicationId;
            System.Diagnostics.Process.Start("http://log4stuff.com/app/" + applicationId);
        }
    }
}
