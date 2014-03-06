using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
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
    }
}