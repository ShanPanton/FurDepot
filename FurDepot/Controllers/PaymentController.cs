using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Stripe;
//using Stripe.Checkout;


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
    }
}