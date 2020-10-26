using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ThuongMaiDienTu.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}