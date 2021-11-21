using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurDepot.Models
{
	public class IncidentContent
	{
		public Incident Incident;
		public User User;

		public bool CurrentUserIsOwner {
			get {
				if (Incident == null) return false;
				if (Incident.User == null) return false;
				if (User == null) return false;
				if (User.UserID != Incident.User.UserID) return false;
				return true;
			}
		}
	}
}