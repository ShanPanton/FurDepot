using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;


namespace FurDepot.Controllers {
	public class IncidentController : Controller {

		// GET: IncidentForm
		public ActionResult IncidentForm() {
			Models.Incident i = new Models.Incident();
			Models.User u = new Models.User();
			u = u.GetUserSession();
			i.User = u;

			if (i.User.IsAuthenticated) {
				if (RouteData.Values["incidentid"] == null) { // add an empty incident
					i.SubmittedDate = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0);
				}
				else {// get the incident
					int IncidentID = Convert.ToInt32(RouteData.Values["IncidentID"]);
					i = i.GetIncident(IncidentID);
				}
			}
			return View(i);
		}


		[HttpPost]
		public ActionResult IncidentForm(FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();

			if (col["btnCancel"] == "cancel") {
				return RedirectToAction("Index", "Home");
			}

			if (col["btnSubmit"] == "view") {
				return RedirectToAction("IncidentRecords", "Incident");
			}

			if (col["btnSubmit"] == "submit") {

				Models.Incident i = new Models.Incident();


				if (RouteData.Values["IncidentID"] != null) i.IncidentID = Convert.ToInt32(RouteData.Values["IncidentID"]);
				i.User = u;

				i.OwnerUserName = col["OwnerUserName"];
				i.FirstName = col["FirstName"];
				i.LastName = col["LastName"];
				i.IncidentTitle = col["IncidentTitle"];
				i.IncidentDesc = col["IncidentDesc"];
				i.UserName = col["UserName"];

				if (i.OwnerUserName.Length == 0 || i.FirstName.Length == 0 || i.LastName.Length == 0 || i.IncidentTitle.Length == 0 || i.IncidentDesc.Length == 0 || i.UserName.Length == 0) {
					i.ActionType = Models.Incident.ActionTypes.RequiredFieldsMissing;
					return View();
				}

				i.Save();
				return RedirectToAction("Submitted");
			}
			//}
			return View();
		}


		//*********************************************************
		// Need help
		//*********************************************************
		//// GET: Incident Records
		//public ActionResult IncidentRecords()
		//{
		//	Models.Incident i = new Models.Incident();
		//	Models.User u = new Models.User();
		//	u = u.GetUserSession();

		//	if (u.IsAuthenticated) {

		//		Models.Database db = new Models.Database();
		//		List<Models.Incident> incidents = new List<Models.Incident>();
		//		incidents = db.GetIncidents(i.IncidentID, u.UserName);
		//	}
		//	return View(i);
		//}


		//[HttpPost]
		//public ActionResult IncidentRecords(FormCollection col)
		//{

		//	if (col["btnClose"] == "close") {
		//		return RedirectToAction("IncidentForm", "Incident");
		//	}

		//	try {
		//		Models.Incident i = new Models.Incident();
		//		Models.User u = new Models.User();
		//		u = u.GetUserSession();

		//		i.IncidentID = Convert.ToInt32(col["IncidentID"]);
		//		i.UserName = col["UserName"];
		//		i.IncidentTitle = col["IncidentTitle"];
		//		i.IncidentDesc = col["IncidentDesc"];
		//		/*i.SubmittedDate = Convert.ToDateTime(col["SubmittedDate"]);*/
		//		//i.Resolved = Convert.ToBoolean(col["Resolved"]);

		//		return View(i);
		//	}
		//	catch (Exception) {
		//		Models.Incident i = new Models.Incident();
		//		return View(i);
		//	}
		//}
		//*********************************************************



		// GET: Submitted
		public ActionResult Submitted() {
			Models.Incident i = new Models.Incident();
			return View(i);
		}

		[HttpPost]
		public ActionResult Submitted(FormCollection col) {
			// close button
			return RedirectToAction("Index", "Home");
		}
	}
}