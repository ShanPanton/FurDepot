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
		


        // GET: Profile; User Account HomePage
        public ActionResult Index() {
			Models.User u = new Models.User();
			u = u.GetUserSession();
		
			return View(u);
        }
		//Addded 11/1/2021 - Hilbert; Users Payment
		public ActionResult Payment() {
			return View();
		}

		//Addded 11/1/2021 - Hilbert ; Users Shipping
		public ActionResult Shipping() {
			return View();
		}

		public ActionResult Orders() {
			return View();
		}

		// Sign up
		public ActionResult SignUp() {
			Models.User u = new Models.User();
			return View(u);
		}


		[HttpPost]
		public ActionResult SignUp(FormCollection col) {
			try {
				Models.User u = new Models.User();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.Phone = col["Phone"];
				u.UserName = col["Username"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Phone.Length == 0 || u.UserName.Length == 0 || u.Password.Length == 0) {
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else {
					if (col["btnSubmit"] == "signup") { //sign up button pressed
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


						switch (at) {
							case Models.User.ActionTypes.InsertSuccessful:
								u.SaveUserSession();
								return RedirectToAction("Index", "Home");
							//break;
							default:
								return View(u);
								//break;
						}
					}
					else {
						return View(u);
					}
				}
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}







		public ActionResult SignOut() {
			Models.User u = new Models.User();
			u.RemoveUserSession();
			return RedirectToAction("Index", "Home");

		}



		//Addded 11/1/2021 - Hilbert 
		public ActionResult SignIn() {
			Models.User u = new Models.User();
			return View(u);
		}



		[HttpPost]
		public ActionResult SignIn(FormCollection col) {
			try {
				Models.User u = new Models.User();

				u.UserName = col["UserName"];
				u.Password = col["Password"];

				if (u.UserName.Length == 0 || u.Password.Length == 0) {
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else {
					if (col["btnSubmit"] == "signin") {
						u.UserName = col["UserName"];
						u.Password = col["Password"];

						u = u.Login();
						if (u != null && u.UserID > 0) {
							u.SaveUserSession();
							return RedirectToAction("Index", "Home");
						}
						else {
							u = new Models.User();
							u.UserName = col["UserName"];
							u.ActionType = Models.User.ActionTypes.LoginFailed;
							return View(u);
						}
					}
					else {
						return View(u);
					}
				}
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}

		// Update Profile
		public ActionResult UpdateProfile() {
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
		public ActionResult UpdateProfile(HttpPostedFileBase UserImage, FormCollection col) {

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

		// Product Gallery
		public ActionResult Gallery() {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				Models.Database db = new Models.Database();
				u.Images = db.GetUserImage(u.UserID);
			}
			return View(u);
		}


		[HttpPost]
		public ActionResult Gallery(IEnumerable<HttpPostedFileBase> files) {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			foreach (var file in files) {
				u.AddGalleryImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		public ActionResult Product() {
			Models.User u = new Models.User();
			Models.Product p = new Models.Product();
			u = u.GetUserSession();
			p.User = u;

			if (p.User.IsAuthenticated) {
				if (RouteData.Values["id"] == null) { //add an empty product
													  //We can set them earlier; this is what affects the start date and end date
					p.DatePost = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
					p.DateSold = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
					p.PurchaseDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				}
				else {
					int id = Convert.ToInt32(RouteData.Values["id"]);
					p = p.GetProduct(id);

				}



				
					
			
				
			}
			return View(p);
		}

		[HttpPost]
		public ActionResult Product(HttpPostedFileBase ProductImage, FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (col["btnSubmit"] == "close") {
				if (col["from"] == null) return RedirectToAction("MyProducts");
				return RedirectToAction("Index", "Home");
			}

			if (col["btnSubmit"] == "event-gallery") {
				return RedirectToAction("EventGallery", new { @ProductID = Convert.ToInt64(RouteData.Values["ProductID"]) });
			}

			if (col["btnSubmit"] == "delete") {
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				return RedirectToAction("DeleteProduct", new { @id = lngID });
			}

			if (col["btnSubmit"] == "save") {

				Models.Product p = new Models.Product();

				if (RouteData.Values["id"] != null) p.ProductID = Convert.ToInt32(RouteData.Values["id"]);
				p.User = u;


				p.ProductName = col["ProductName"];
				if (col["IsActive"].ToString().Contains("true")) p.IsActive = true; else p.IsActive = false;

				p.ProductDesc = col["ProductDesc"];

				p.DatePost = DateTime.Parse(string.Concat(col["DatePost"].ToString()," ", col["DatePost.TimeOfDay"]));

				p.DateSold = DateTime.Parse(string.Concat(col["DateSold"].ToString()," ", col["DatePost.TimeOfDay"]));

				p.PurchaseDate = DateTime.Parse(string.Concat(col["PurchaseDate"].ToString(), " ", col["PurchaseDate.TimeOfDay"]));

				p.ProductCost = Decimal.Parse(string.Concat(col["ProductCost"].ToString()));
					
					//Decimal.Parse(string.Concat(col["ProductCost"].ToString(),"{0:C}", col["ProductCost"]));

				

				if (p.ProductName.Length == 0 || p.ProductDesc.Length == 0 || p.ProductCost == 0) {
					p.ActionType = Models.Product.ActionTypes.RequiredFieldsMissing;
					return View(p);
				}

				p.Save();

				if (ProductImage != null) {
					p.ProductImage = new Models.Image();
					if (col["ProductImage.ImageID"].ToString() == "") {
						p.ProductImage.ImageID = 0;
					}
					else {
						p.ProductImage.ImageID = Convert.ToInt32(col["ProductImage.ImageID"]);
					}

					p.ProductImage.Primary = true;
					p.ProductImage.FileName = Path.GetFileName(ProductImage.FileName);
					if (p.ProductImage.IsImageFile()) { // Getting properties of that file
						p.ProductImage.ImageSize = ProductImage.ContentLength;
						Stream stream = ProductImage.InputStream;
						BinaryReader binaryReader = new BinaryReader(stream);
						p.ProductImage.ImageData = binaryReader.ReadBytes((int)stream.Length);

						p.UpdatePrimaryImage();
					}
				}

				if (p.ProductID > 0) {
					return RedirectToAction("Product", new {@id = p.ProductID });
				}
			}
			return View();
		}


		public ActionResult MyProducts() {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated)
				u.Products = u.GetProducts();
			return View(u);

		}

		//Change ID into ProductID; naming so it can be more consistent
		[HttpPost]
		public JsonResult DeleteProductImage (int UserID, int ProductID) {
			try {
				string type = string.Empty;
				Models.Database db = new Models.Database();
				if (db.DeleteProductImage(ProductID)) return Json(new { Status = 1 });
				return Json(new { Status = 0 });
			}
			catch(Exception ex) {
				return Json(new { Status = -1 });
			}
		}


		public ActionResult DeleteProduct() {
			Models.Product p = new Models.Product();
			p.User = new Models.User();
			p.User = p.User.GetUserSession();
			if (p.User.IsAuthenticated) {
				int intID = Convert.ToInt32(RouteData.Values["id"]);
				p = p.GetProduct(intID);
			}
			return View(p);

		}

		[HttpPost]
		public ActionResult DeleteProduct(FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				int intID = Convert.ToInt32(RouteData.Values["id"]);

				if (col["btnSubmit"] == "close") return RedirectToAction("Product", new { @id = intID });
				if (col["btnSubmit"] == "delete") {
					Models.Database db = new Models.Database();
					db.DeleteProduct(intID);
				}
			}
			return RedirectToAction("MyProducts"); //this should never happen
		}




	}


}