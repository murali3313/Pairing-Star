using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PairingStar.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Pairing Star! An efficient tool for maintaining pairing trend in an Agile project.";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
