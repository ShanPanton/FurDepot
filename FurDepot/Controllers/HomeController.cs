using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FurDepot.Controllers {
	public class HomeController : Controller {


		// GET: Home
		public ActionResult Index() {
			// Access Models for Database and HomeContent and set new variables
			Models.Database db = new Models.Database();
			Models.HomeContent hc = new Models.HomeContent();

			// Get Product Data
			hc.Products = new List<Models.Product>();
			hc.Products = db.GetActiveProducts();

			// Get User Data
			hc.User = new Models.User();
			hc.User = hc.User.GetUserSession();

			// Check if User is signed in?
			if (hc.User.IsAuthenticated) {
				// Yes, User has access to rating
				hc.User.Ratings = db.GetProductRatings(hc.User.UserID);
			}

			return View(hc);
		}



		// GET: Product / Shop
		public ActionResult Product(/*int ProductID*/) {
			//// Access to Models for Database and Product Content and set new variables
			//Models.Database db = new Models.Database();
			//Models.ProductContent pc = new Models.ProductContent();

			//pc.User = new Models.User();
			//pc.User = pc.User.GetUserSession();

			//if (pc.User.IsAuthenticated) {
			//	pc.User.Ratings = db.GetProductRatings(pc.User.UserID);
			//}

			//// Need to check what this is for.
			//long id = Convert.ToInt64(RouteData.Values["productid"]);
			//pc.Product = new Models.Product();
			//pc.Product = pc.Product.GetProduct(ProductID);

			return View(/*pc*/);
		}


		// Added 10.30.2021 Mai
		[HttpPost]
		public ActionResult Product(FormCollection col) {
			//close button
			return RedirectToAction("Index");
		}


		//[HttpPost]
		//public JsonResult ProductRate(int UserID, int ProductID, int RateID)
		//{
		//	try {
		//		// Access Models for Access and set new variables
		//		Models.Database db = new Models.Database();
		//		int intReturn = 0;
		//		intReturn = db.RateProduct(UserID, ProductID, RateID);
		//		return Json(new { Status = intReturn });
		//	}
		//	catch (Exception ex) {
		//		return Json(new { Status = -1 }); //error
		//	}
		//}


		// Added 11.6.2021 Mai
		public ActionResult Incident() {
			Models.Database db = new Models.Database();
			Models.User u = new Models.User();
			Models.IncidentContent ic = new Models.IncidentContent();

			ic.User = new Models.User();
			ic.User = ic.User.GetUserSession();


			if (ic.User.IsAuthenticated) {
				ic.User.Ratings = db.GetProductRatings(ic.User.UserID);
			}

			//int IncidentID = Convert.ToInt64(RouteData.Values["IncidentID"]);
			//ic.Incident = new Models.Incident();

			return View(ic);
		}



		//****************************************************************
		// These may be able to delete
		// ***************************************************************
		//[HttpPost]
		//public ActionResult Incident(FormCollection col)
		//{
		//	// close button
		//	return RedirectToAction("Index");
		//}


		// Added 10.29.2021
		// this is for registered user to submit incident directly from the product page
		//[HttpPost]
		//public JsonResult SaveIncident(int UserSubmittingIncidentID, int UserIncidentIsAboutID, int IncidentTypeID)
		//{
		//	try {
		//		Models.Database db = new Models.Database();
		//		System.Threading.Thread.Sleep(3000);
		//		bool b = false;
		//		b = db.InsertIncident(UserSubmittingIncidentID, UserIncidentIsAboutID, IncidentTypeID);
		//		return Json(new { Status = b });
		//	}
		//	catch (Exception ex) {
		//		return Json(new { Status = -1 }); //error
		//	}
		//}
		// ***************************************************************


		// GET: Terms
		public ActionResult Terms() {
			return View();
		}


		// GET: Help - Added on 11.9.2021 Mai
		public ActionResult Help() {
			return View();
		}


		// GET: Shipping & Returns
		public ActionResult Shipping() {
			return View();
		}

		// GET: FAQ
		public ActionResult FAQ() {
			return View();
		}

		// GET: Store Policy
		public ActionResult StorePolicy() {
			return View();
		}

		// GET: About
		public ActionResult About() {
			return View();
		}


		// GET: Contact
		public ActionResult Contact() {
			return View();
		}

		// GET: Privacy Statement
		public ActionResult PrivacyStatement() {
			return View();
		}


		// GET: Disclosure
		public ActionResult Disclosure() {
			return View();
		}


	}
}




