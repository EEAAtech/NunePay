using System.Web.Mvc;
using System.Data.Entity;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss, HR, Anon")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
        public ActionResult pli()
        {
            return View();
        }
    }
}