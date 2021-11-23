using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailKit.Net.Smtp;
using MimeKit;


namespace FurDepot.Controllers
{
    public class ProfileController : Controller
    {
		// GET: Profile
		public ActionResult Index()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();

			return View(u);
		}


		// Sign up
		public ActionResult SignUp()
		{
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignUp(FormCollection col)
		{
			try 
			{
				Models.User u = new Models.User();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.Phone = col["Phone"];
				u.UserName = col["Username"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Phone.Length == 0 || u.UserName.Length == 0 || u.Password.Length == 0) 
				{
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else 
				{
					if (col["btnSubmit"] == "signup") 
					{ //sign up button pressed
						Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
						at = u.Save();

						// ***************************************************************************************************
						// After saved the user information, sending an email notification using Gmail SMTP - Added on 11.15 - Mai
						// ***************************************************************************************************
						using (var client = new SmtpClient()) {
							client.Connect("smtp.gmail.com");
							client.Authenticate("furdepottest2021@gmail.com", "theateam2021");

							var bodyBuilder = new BodyBuilder {
								HtmlBody = $"<p>Thank you for joining the Fur Depot, {u.FirstName}!</p> <p>&nbsp;</p> <p>Here is your username and password:</p> <p>&nbsp;&nbsp;Username: {u.UserName}</p> <p>&nbsp;&nbsp;Password: {u.Password}</p> <p>Enjoy the second-hand experience with the Fur Depot!</p>",
								TextBody = "{u.FirstName} \r\n {u.UserName} \r\n {u.Password}"
							};

							var message = new MimeMessage {
								Body = bodyBuilder.ToMessageBody()
							};
							message.From.Add(new MailboxAddress("Noreply from The Fur Depot", "furdepottest2021@gmail.com"));
							message.To.Add(new MailboxAddress("New Fur Depot Family!", u.Email));
							message.Bcc.Add(new MailboxAddress("Admin Copy", "furdepottest2021@gmail.com"));
							message.Subject = "Welcome to the Fur Depot!";
							client.Send(message);

							client.Disconnect(true);
						}
						// **********************************************************************************


						switch (at) 
						{
							case Models.User.ActionTypes.InsertSuccessful:
								u.SaveUserSession();
								return RedirectToAction("Index", "Home");
							//break;
							default:
								return View(u);
								//break;
						}
					}
					else 
					{
						return View(u);
					}
				}
			}
			catch (Exception) 
			{
				Models.User u = new Models.User();
				return View(u);
			}
		}




		// Sign in
		public ActionResult SignIn()
		{
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignIn(FormCollection col)
		{
			try 
			{
				Models.User u = new Models.User();

				u.UserName = col["UserName"];
				u.Password = col["Password"];

				if (u.UserName.Length == 0 || u.Password.Length == 0)
				{
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else 
				{
					if (col["btnSubmit"] == "signin") 
					{
						u.UserName = col["UserName"];
						u.Password = col["Password"];

						u = u.Login();
						if (u != null && u.UserID > 0) 
						{
							u.SaveUserSession();
							return RedirectToAction("Index", "Home");
						}
						else 
						{
							u = new Models.User();
							u.UserName = col["UserName"];
							u.ActionType = Models.User.ActionTypes.LoginFailed;
							return View(u);
						}
					}
					else 
					{
						return View(u);
					}
				}
			}
			catch (Exception) 
			{
				Models.User u = new Models.User();
				return View(u);
			}
		}



		// Sign out
		public ActionResult SignOut()
		{
			Models.User u = new Models.User();
			u.RemoveUserSession();
			return RedirectToAction("Index", "Home");
		}





		// Update Profile
		public ActionResult UpdateProfile()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();

			if (u.IsAuthenticated) {
				Models.Database db = new Models.Database();
				List<Models.Image> images = new List<Models.Image>();
				images = db.GetUserImage(u.UserID, 0, true);
				u.UserImage = new Models.Image();
				if (images.Count > 0) u.UserImage = images[0];
			}
			return View(u);
		}

		[HttpPost]
		public ActionResult UpdateProfile(HttpPostedFileBase UserImage, FormCollection col)
		{

			try {
				Models.User u = new Models.User();
				u = u.GetUserSession();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.Phone = col["Phone"];
				u.UserName = col["UserName"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Phone.Length == 0 || u.UserName.Length == 0 || u.Password.Length == 0) {
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else {
					if (col["btnSubmit"] == "update") { //update button pressed
						u.Save();

						u.UserImage = new Models.Image();
						u.UserImage.ImageID = System.Convert.ToInt32(col["UserImage.ImageID"]);


						// ************************************************************************************************************
						// After saved the user information, sending an email notification using Gmail SMTP - Added on 11.15 - Mai
						// ************************************************************************************************************
						using (var client = new SmtpClient()) {
							client.Connect("smtp.gmail.com");
							client.Authenticate("furdepottest2021@gmail.com", "theateam2021");

							var bodyBuilder = new BodyBuilder {
								HtmlBody = $"<p>You have successfully updated your account information.</p> <p>Here is your current profile information:</p> <p>&nbsp;&nbsp;Full Name: {u.FirstName}&nbsp;{u.LastName}</p> <p>&nbsp;&nbsp;Email: {u.Email}</p> <p>&nbsp;&nbsp;Phone: {u.Phone}</p> <p>&nbsp;&nbsp;Username: {u.UserName}</p> <p>&nbsp;&nbsp;Password: {u.Password}</p> <p>&nbsp;</p> <p>Thank you for using the Fur Depot!</p>",
								TextBody = "{u.FirstName} {u.LastName}\r\n {u.Email} \r\n {u.Phone} \r\n {u.UserName} \r\n {u.Password}"
							};

							var message = new MimeMessage {
								Body = bodyBuilder.ToMessageBody()
							};
							message.From.Add(new MailboxAddress("Noreply from The Fur Depot", "furdepottest2021@gmail.com"));
							message.To.Add(new MailboxAddress("The Fur Depot Family!", u.Email));
							message.Bcc.Add(new MailboxAddress("Admin Copy", "furdepottest2021@gmail.com"));
							message.Subject = "The Fur Depot: Your account information has been updated!";
							client.Send(message);

							client.Disconnect(true);
						}
						// **********************************************************************************


						if (UserImage != null) {
							u.UserImage = new Models.Image();
							u.UserImage.ImageID = Convert.ToInt32(col["UserImage.ImageID"]);
							u.UserImage.Primary = true;
							u.UserImage.FileName = Path.GetFileName(UserImage.FileName);

							if (u.UserImage.IsImageFile()) {
								u.UserImage.ImageSize = UserImage.ContentLength;
								Stream stream = UserImage.InputStream;
								BinaryReader binaryReader = new BinaryReader(stream);
								u.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
								u.UpdatePrimaryImage();
							}
						}
						u.SaveUserSession();
						return RedirectToAction("Index", "Profile");
					}
					return View(u);
				}
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}

		}


		// Gallery
		public ActionResult Gallery()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				Models.Database db = new Models.Database();
				u.Images = db.GetUserImage(u.UserID);
			}
			return View(u);
		}


		[HttpPost]
		public ActionResult Gallery(IEnumerable<HttpPostedFileBase> files)
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			foreach (var file in files) {
				u.AddGalleryImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		//*************************************************************************************
		// Added Hilbert data on 11.12.2021 - Mai
		//*************************************************************************************

		//Addded 11/1/2021 - Hilbert; Users Payment
		public ActionResult Payment()
		{
			return View();
		}

		//Addded 11/1/2021 - Hilbert ; Users Shipping
		public ActionResult Shipping()
		{
			return View();
		}

		public ActionResult Orders()
		{
			return View();
		}


		// Added for Incident Records on 11.17 - Mai
		public ActionResult MyTransactions()
		{
			return View();
		}

	}
}