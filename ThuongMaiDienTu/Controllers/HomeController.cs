using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ThuongMaiDienTu.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Giá không tưởng";
            return View();
        }

        public ActionResult Category(int id)
        {
            return View();
        }
    }
}
