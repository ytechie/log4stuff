using System;
using System.Net;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Microsoft.LogHub.Controllers
{
    public class AppsController : Controller
    {
        //
        // GET: /Apps/
        public ActionResult Index(string id)
        {
            ViewBag.ApplicationId = id;
            return View();
        }

        public ActionResult Configure(string id)
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

            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }

            ViewBag.NewGuid = id;

            return View();
        }
    }
}