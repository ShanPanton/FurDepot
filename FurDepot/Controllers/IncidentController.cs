using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MailKit.Net.Smtp;
using MimeKit;
using FurDepot.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace FurDepot.Controllers
{
    public class IncidentController : Controller
    {

        // Added for Incident Records on 11.17 - Mai
        // ***************************************************************
        // Need to be modified for authentication
        // ***************************************************************
        // GET: IncidentRecords
        public ActionResult IncidentRecords(DisplayIncidents di)
        {

            Models.Incident i = new Models.Incident();
            Models.User u = new Models.User();
            u = u.GetUserSession();
            i.User = u;

            // I copy and pasted our database functions at the bottom of this class. At the time I was having
            // problems calling them from Model/Database.cs. I'm using them instead of using SqlConnection object.
            // I'm also using SqlDataAdapter here instead of using SqlDataReader. I created an else condition 
            // in IncidentRecords.cshtml to display a message on the website that the user has no incidents instead 
            // of breaking the website. 

            SqlConnection cn = null;
            if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");

            // Notice the single quotes appended around u.UserName. Honestly not sure why this is the only method that seems to work.
            string s1 = "select * from [db_owner].[TIncidents] where TIncidents.OwnerUserName='" + u.UserName + "'";
            SqlCommand sqlcomm = new SqlCommand(s1, cn);
            SqlDataAdapter sdr = new SqlDataAdapter();
            sdr.SelectCommand = sqlcomm;
            DataSet ds = new DataSet();

            try
            {
                sdr.Fill(ds);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Incident Report Error: " + ex.Message);
            }
            finally { CloseDBConnection(ref cn); }

            List<DisplayIncidents> objmodel = new List<DisplayIncidents>();


            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var details = new DisplayIncidents();
                    details.IncidentID = dr["IncidentID"].ToString();
                    details.OwnerUserName = dr["OwnerUserName"].ToString();
                    details.FirstName = dr["FirstName"].ToString();
                    details.LastName = dr["LastName"].ToString();
                    details.IncidentTitle = dr["IncidentTitle"].ToString();
                    details.IncidentDesc = dr["IncidentDesc"].ToString();
                    details.UserName = dr["UserName"].ToString();
                    details.SubmittedDate = dr["SubmittedDate"].ToString();
                    details.Resolved = dr["Resolved"].ToString();
                    objmodel.Add(details);
                }
                di.incidentinfo = objmodel;
            }
            return View("IncidentRecords", di);
        }


        // GET: IncidentForm
        public ActionResult IncidentForm()
		{
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


		// *****************************************************************************
		// This subroutine can be used for admins to view all incident reports logged.
		// *****************************************************************************
		// GET: IndividualIncidentRecords
		public ActionResult IncidentRecordsLog(DisplayIncidents di)
		{

			string mainconn = ConfigurationManager.AppSettings["AppDBConnect"]/*.ConnectionString*/;
			SqlConnection sqlconn = new SqlConnection(mainconn);
			string s1 = "select * from [db_owner].[TIncidents]";
			SqlCommand sqlcomm = new SqlCommand(s1);
			sqlcomm.Connection = sqlconn;
			sqlconn.Open();
			SqlDataReader sdr = sqlcomm.ExecuteReader();

			List<DisplayIncidents> objmodel = new List<DisplayIncidents>();

			if (sdr.HasRows)
			{
				while (sdr.Read())
				{
					var details = new DisplayIncidents();
					details.IncidentID = sdr["IncidentID"].ToString();
					details.OwnerUserName = sdr["OwnerUserName"].ToString();
					details.FirstName = sdr["FirstName"].ToString();
					details.LastName = sdr["LastName"].ToString();
					details.IncidentTitle = sdr["IncidentTitle"].ToString();
					details.IncidentDesc = sdr["IncidentDesc"].ToString();
					details.UserName = sdr["UserName"].ToString();
					details.SubmittedDate = sdr["SubmittedDate"].ToString();
					details.Resolved = sdr["Resolved"].ToString();
					objmodel.Add(details);
				}									
				di.incidentinfo = objmodel;
				sqlconn.Close();
			}
			return View("IncidentRecordsLog", di);
		}



		[HttpPost]
		public ActionResult IncidentForm(FormCollection col)
		{
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

				if (i.OwnerUserName.Length == 0 || i.FirstName.Length == 0 || i.LastName.Length == 0 || i.IncidentTitle.Length == 0 || i.IncidentDesc.Length == 0 || i.UserName.Length == 0 ) {
					i.ActionType = Models.Incident.ActionTypes.RequiredFieldsMissing;
					return View();
				}




				// ************************************************************************************************************
				// After submitted the incident information, sending an email notification using Gmail SMTP - Added on 11.15 - Mai
				// ************************************************************************************************************
				using (var client = new SmtpClient()) {
					client.Connect("smtp.gmail.com");
					client.Authenticate("furdepottest2021@gmail.com", "theateam2021");

					var bodyBuilder = new BodyBuilder {
						HtmlBody = $"<p>Thank you for submitting the incident form.</p> <p>&nbsp;</p> <p>Detailed information:</p> <p>&nbsp;&nbsp;Full Name: {i.FirstName}&nbsp;{i.LastName}</p> <p>&nbsp;&nbsp;Your Username: {i.OwnerUserName}</p> <p>&nbsp;&nbsp;Incident Title: {i.IncidentTitle}</p> <p>&nbsp;&nbsp;Username you want to report about: {i.UserName}</p> <p>&nbsp;&nbsp;Incident Description: {i.IncidentDesc}</p> <p>&nbsp;</p> <p>One of our staff will reach out to you as soon as it is reviewed.</p>",
						TextBody = "{i.FirstName} {i.LastName}\r\n {i.OwnerUserName} \r\n {i.IncidentTitle} \r\n {i.UserName} \r\n {i.IncidentDesc}"
					};

					var message = new MimeMessage {
						Body = bodyBuilder.ToMessageBody()
					};
					message.From.Add(new MailboxAddress("Noreply from The Fur Depot", "furdepottest2021@gmail.com"));
					message.To.Add(new MailboxAddress("The Fur Depot Family!", u.Email));
					message.Bcc.Add(new MailboxAddress("Admin Copy", "furdepottest2021@gmail.com"));
					message.Subject = "The Fur Depot: We have received your incident form.";
					client.Send(message);

					client.Disconnect(true);
				}
				// **********************************************************************************




				i.Save();
				return RedirectToAction("Submitted");
				}
			//}
			return View();
		}



		// GET: Submitted
		public ActionResult Submitted()
		{
			Models.Incident i = new Models.Incident();
			return View(i);
		}

		[HttpPost]
		public ActionResult Submitted(FormCollection col)
		{
			// close button
			return RedirectToAction("Index", "Home");
		}


			// GET: SQL Connection
	private bool GetDBConnection(ref SqlConnection SQLConn)
	{
		try
		{
			if (SQLConn == null) SQLConn = new SqlConnection();
			if (SQLConn.State != ConnectionState.Open)
			{
				SQLConn.ConnectionString = ConfigurationManager.AppSettings["AppDBConnect"];
				SQLConn.Open();
			}
			return true;
		}
		catch (Exception ex) { throw new Exception(ex.Message); }
	}



	// CLOSE: SQL Connection
	private bool CloseDBConnection(ref SqlConnection SQLConn)
	{
		try
		{
			if (SQLConn.State != ConnectionState.Closed)
			{
				SQLConn.Close();
				SQLConn.Dispose();
				SQLConn = null;
			}
			return true;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	}

}

