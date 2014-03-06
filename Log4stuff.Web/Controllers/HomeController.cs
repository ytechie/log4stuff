using System;
using System.Web.Mvc;

namespace Log4stuff.Web.Controllers
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
            return RedirectToAction("Configure", "Apps", new {id});
        }
    }
}