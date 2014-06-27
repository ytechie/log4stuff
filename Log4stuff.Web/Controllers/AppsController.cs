using System;
using System.Net;
using System.Web.Mvc;
using Log4stuff.Web.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Log4stuff.Web.Controllers
{
    public class AppsController : Controller
    {
        //
        // GET: /Apps/
        public ActionResult Index(string id, string logEvent = null)
        {
            if (logEvent == null)
            {
                ViewBag.ApplicationId = id;
                return View();
            }

            var l = new LogEvent();
            l.PopulateFromJson(logEvent);
            l.ApplicationId = id;

            var context = GlobalHost.ConnectionManager.GetHubContext<LogMessageHub>();
            context.Clients.Group(id).newLogMessage(logEvent);

            return new HttpStatusCodeResult(200);
        }

        public ActionResult Configure(string id)
        {
            string ip = "ENTER_WEBSITE_IP";
            if (RoleEnvironment.IsAvailable)
            {
                if (Request.Url != null)
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(Request.Url.DnsSafeHost);
                    if (addresses.Length > 0)
                        ip = addresses[0].ToString();
                }
            }
            else
            {
                ip = Request.ServerVariables.Get("LOCAL_ADDR");
            }
            ViewBag.IpAddress = ip;

            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }

            ViewBag.NewGuid = id;
            ViewBag.QrCodeUrl = "https://chart.googleapis.com/chart?chs=150x150&cht=qr&chl=http%3A%2F%2Flog4stuff.com" +
                                Url.Encode(Url.Action("index", "Apps", new { id }));

            return View();
        }
    }
}