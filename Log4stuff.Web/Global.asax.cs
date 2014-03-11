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

                byte[] buffer = received.Buffer;

                string xmlText = Encoding.UTF8.GetString(buffer);
                string json = ConvertLog4JToJson(xmlText);

                //This should be in the hub wrapper
                string applicationId = GetApplicationId(xmlText);
                if (applicationId != null)
                    context.Clients.Group(applicationId).newLogMessage(json);
            }
        }

        public static string GetApplicationId(string xml)
        {
            const string Prefix = "<log4j:data name=\"ApplicationId\" value=\"";

            try
            {
                int start = xml.IndexOf(Prefix);
                if (start == -1)
                    return null;
                start += Prefix.Length;

                int end = xml.IndexOf('\"', start + 1);

                return xml.Substring(start, end - start);
            }
            catch (Exception)
            {
                //Just eat exceptions to deal with malformed data
                return null;
            }
        }

        //Xml parsing in .NET sucks, convert it to JSON and let the client process it
        public static string ConvertLog4JToJson(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml.Replace("log4j:", ""));

            string jsonText = JsonConvert.SerializeXmlNode(doc);

            return jsonText;
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
                        log.Debug("This is a really long simulated message that would never fit int a single row. Notice that it's truncated.");
                        break;
                    default:
                        log.Debug("This is a debug message");
                        break;
                }
                
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        }
    }
}