using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Microsoft.LogHub
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StartUdpServer();
        }

        private async void StartUdpServer()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, 0);
            var udp = new UdpClient(8080);

            var context = GlobalHost.ConnectionManager.GetHubContext<LogMessageHub>();

            while (true)
            {
                var received = await udp.ReceiveAsync();
                var buffer = received.Buffer;

                var xmlText = System.Text.Encoding.UTF8.GetString(buffer);
                var json = ConvertLog4JToJson(xmlText);

                //This should be in the hub wrapper
                context.Clients.All.newLogMessage(json);
            }
        }

        //Xml parsing in .NET sucks, convert it to JSON and let the client process it
        public static string ConvertLog4JToJson(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml.Replace("log4j:", ""));

            var jsonText = JsonConvert.SerializeXmlNode(doc);

            return jsonText;
        }
    }
}
