using System;
using System.Web.Mvc;
using System.Web.UI;

namespace Log4stuff.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.NewGuid = Guid.NewGuid().ToString();

            return View();
        }

        //[OutputCache(Duration = 600, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        public ActionResult About()
        {
            return View();
        }

        // [OutputCache(Duration = 600, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        public ActionResult Pricing()
        {
            return View();
        }

        public ActionResult LearnMore()
        {
            return View();
        }

        public ActionResult GetTheCode(string id)
        {
            return RedirectToAction("Configure", "Apps", new { id });
        }
    }
}