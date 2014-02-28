using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.Owin.BuilderProperties;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Microsoft.LogHub.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string id)
        {
            ViewBag.ApplicationId = id;
            return View();
        }

        public ActionResult About()
        {
            var ip = "ENTER_WEBSITE_IP";
            if (RoleEnvironment.IsAvailable)
            {
                if (Request.Url != null)
                {
                    var addresses = Dns.GetHostAddresses(Request.Url.DnsSafeHost);
                    if (addresses.Length > 0)
                        ip = addresses[0].ToString();
                }
            }
            else
            {
                ip = Request.ServerVariables.Get("LOCAL_ADDR");
            }
            ViewBag.IpAddress = ip;
            ViewBag.NewGuid = Guid.NewGuid().ToString();
            return View();
        }
    }
}