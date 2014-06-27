using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using Log4stuff.Web.App_Start;
using Log4stuff.Web.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Log4stuff.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StartUdpServer();
            StartLogSimulator();
        }

        private async void StartUdpServer()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, 0);
            var udp = new UdpClient(8080);

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<LogMessageHub>();

            while (true)
            {
                UdpReceiveResult received = await udp.ReceiveAsync();

                //TODO: Are we still listening for UDP messages at this point?

                var buffer = received.Buffer;

                var xmlText = Encoding.UTF8.GetString(buffer);
                var logEvent = new LogEvent();
                logEvent.PopulateFromLog4JXml(xmlText);

                var json = logEvent.ToJson();

                //This should be in the hub wrapper
                var applicationId = logEvent.ApplicationId;
                if (applicationId != null)
                    context.Clients.Group(applicationId).newLogMessage(json);
            }
        }

        private static Timer SimulatorTimer;

        private static void StartLogSimulator()
        {
            var traceAppender = new TraceAppender { Layout = new PatternLayout() };

            var udpAppender = new UdpAppender
            {
                RemoteAddress = IPAddress.Loopback,
                RemotePort = 8080,
                Name = "UDPAppender",
                Encoding = new ASCIIEncoding(),
                Threshold = Level.Debug,
                Layout = new XmlLayoutSchemaLog4j()
            };
            udpAppender.ActivateOptions();

            BasicConfigurator.Configure(traceAppender, udpAppender);

            var log = LogManager.GetLogger("Simulator");
            log4net.GlobalContext.Properties["ApplicationId"] = "log4stuff";

            var rand = new Random();
            SimulatorTimer = new Timer(state =>
            {
                var option = rand.Next(0, 3); //max is exclusive
                switch (option)
                {
                    case 0:
                        log.Info("This is an info message");
                        break;
                    case 1:
                        log.Debug("This is a really long message that will be truncated. It would never fit into a single row.");
                        break;
                    default:
                        log.Debug("This is a debug message");
                        break;
                }

            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        }
    }
}