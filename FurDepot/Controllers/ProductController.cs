using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FurDepot.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product

        public ActionResult ProductDetail(int productId)
        {
            // get a uhhhh product from the uhhh database by its id
            Models.Product p = new Models.Product();
            Models.Database db = new Models.Database();
            p = db.GetProductDetails(productId);

            // - lets make a stored procedure for this tho
            // pass that bad boy to the View
            // profit
            return View(p);
        }
    }
}