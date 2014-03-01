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
        public ActionResult Index()
        {
            ViewBag.NewGuid = Guid.NewGuid().ToString();

            return View();
        }

        public ActionResult About()
        {
            return View();

        }

        public ActionResult GetTheCode(string id)
        {
            return RedirectToAction("Configure", "Apps", new { id = id });
        }
    }
}