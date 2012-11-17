using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models.Home;
using UptimeData;
using StackExchange.Profiling;

namespace Site.Controllers
{
    public class HomeController : Controller
    {
        private UptimeDB DB
        {
            get { return new UptimeDB(); }
        }

        [OutputCache(Duration = 30)]
        public ActionResult Index()
        {
            var profiler = MiniProfiler.Current;
            using (profiler.Step("Index"))
            {
                var model = new IndexViewModel(DB);

                return View(model);
            }
        }

        public ActionResult About()
        {
            return View();
        }

        [OutputCache(Duration = 30)]
        public JsonResult UptimeData()
        {
            var model = new UptimeDataJsonModel(DB);
            model.Fill();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecentPolls()
        {
            var model = new RecentPollsViewModel(DB);
            return View(model);
        }

    }
}
