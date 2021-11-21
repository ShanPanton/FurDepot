using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FurDepot.Models {
	public class Incident {

		public int IncidentID = 0;
		public string OwnerUserName = null;
		public string FirstName = null;
		public string LastName = null;
		public string IncidentTitle = null;
		public string IncidentDesc = null;
		public string UserName = null;
		public DateTime SubmittedDate;
		public bool Resolved = false;

		public User User;
		public ActionTypes ActionType = ActionTypes.RequiredFieldsMissing;


		// Save Incident
		public Incident.ActionTypes Save() {
			try {
				Database db = new Database();

				if (IncidentID == 0) { //insert new user
					this.ActionType = db.InsertIncident(this);
				}
				return this.ActionType;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}

		// Added 11.6.2021 Mai
		// Get Incident
		public Incident GetIncident(int IncidentID) {
			try {
				Database db = new Database();
				List<Incident> incidents = new List<Incident>();
				if (this.User == null) {
					incidents = db.GetIncidents(IncidentID, OwnerUserName);
				}
				else {
					incidents = db.GetIncidents(IncidentID, OwnerUserName);
				}
				return incidents[0];
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}



		//public bool Editable
		//{
		//	get {
		//		if (this.SubmittedDate == null) return true;
		//		if (this.SubmittedDate > DateTime.Now) return true;
		//		return false;
		//	}
		//}


		public enum ActionTypes {
			NoType = 0,
			InsertSuccessful = 1,
			UpdateSuccessful = 2,
			DuplicateEmail = 3,
			DuplicateUserID = 4,
			Unknown = 5,
			RequiredFieldsMissing = 6
		}



	}
}