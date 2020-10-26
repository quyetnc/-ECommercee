using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ThuongMaiDienTu.Controllers
{
    public class ProductController : Controller
    {

        // GET: Product

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Info(int? id)
        {
            using (THUONGMAIDIENTUEntities db = new THUONGMAIDIENTUEntities())
            {
                if (id is null) return HttpNotFound();
                var product = db.PRODUCTs.Where(x => x.IdProduct == id).FirstOrDefault();
                if (product is null) return HttpNotFound();
                return View(product);

            }
        }
    }
}