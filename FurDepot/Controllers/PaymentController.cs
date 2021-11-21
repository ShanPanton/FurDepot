using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stripe;
using Stripe.Checkout;


//Everything created by hilbert
namespace FurDepot.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Shipping() {

			return View();
		}


		public ActionResult Checkout() {
			return View();
		}

		//Don't need a whole view
		public ActionResult Success() {

			return View();
		}

		//Cart Added By Shanique 11/18/2021
		public ActionResult Cart() {

			return View();
		}

		[HttpPost]
		public ActionResult GetCartItems() {
			// ToDo get products from db using products ids arrray/object arg
			// assemble a CartItem model with name, quantity, price, product page url, etc
			return Json(new { yee = "haw" });
		}
	}
}