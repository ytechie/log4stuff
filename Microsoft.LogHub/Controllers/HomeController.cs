using System.Web.Mvc;

namespace Microsoft.LogHub.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.IpAddress = Request.ServerVariables.Get("LOCAL_ADDR");
            return View();
        }
    }
}