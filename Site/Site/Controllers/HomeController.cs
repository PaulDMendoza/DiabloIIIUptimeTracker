using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models.Home;
using UptimeData;

namespace Site.Controllers
{
    public class HomeController : Controller
    {
        private UptimeDB DB
        {
            get { return new UptimeDB(); }
        }

        public ActionResult Index()
        {
            var model = new IndexViewModel(DB);

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }
        
        public JsonResult UptimeData()
        {
            var model = new UptimeDataJsonModel(DB);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecentPolls()
        {
            var model = new RecentPollsViewModel(DB);
            return View(model);
        }

    }
}
