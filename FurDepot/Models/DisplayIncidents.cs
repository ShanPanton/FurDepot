using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurDepot.Models
{
	public class DisplayIncidents
	{
		public string IncidentID { get; set; }
		public string OwnerUserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string IncidentTitle { get; set; }
		public string IncidentDesc { get; set; }
		public string UserName { get; set; }
		public string SubmittedDate { get; set; }
		public string Resolved { get; set; }

		public List<DisplayIncidents> incidentinfo { get; set; }
	}

}