using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FurDepot.Models {
	public class User {
		public int UserID = 0;
		public string FirstName = string.Empty;
		public string LastName = string.Empty;
		public string UserName = string.Empty;
		public string Password = string.Empty;
		public string Email = string.Empty;
		public string Phone = string.Empty;
		public ActionTypes ActionType = ActionTypes.NoType;

		public Image UserImage;
		public List<Image> Images;
		public List<Product> Products = new List<Product>();
		public List<Rating> Ratings;
		public List<Incident> Incidents;


		// User logged in or not by boolean
		public bool IsAuthenticated {
			get {
				if (UserID > 0) return true;
				return false;
			}
		}



		// LOGIN
		public User Login() {
			try {
				Database db = new Database();
				return db.Login(this);
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		// SAVE: Users
		public User.ActionTypes Save() {
			try {
				Database db = new Database();

				if (UserID == 0) { //insert new user
					this.ActionType = db.InsertUser(this);
				}
				else {
					this.ActionType = db.UpdateUser(this);
				}
				return this.ActionType;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		// GET: Product
		public List<Product> GetProducts(int ProductID = 0) {
			try {
				Database db = new Database();
				return db.GetProducts(ProductID, this.UserID);
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}


		// GET: Product Rating
		public byte GetUserRating(Rating.Types RatingType, int ProductID) {
			try {
				foreach (Rating r in this.Ratings) {
					if (r.Type == RatingType && r.ProductID == ProductID) return r.intRating;
				}
				return 0;
			}
			catch (Exception) { return 0; }
		}



		// ADD: Images
		public sbyte AddGalleryImage(HttpPostedFileBase f) {
			try {
				this.UserImage = new Image();
				this.UserImage.Primary = false;
				this.UserImage.FileName = Path.GetFileName(f.FileName);

				if (this.UserImage.IsImageFile()) {
					this.UserImage.ImageSize = f.ContentLength;
					Stream stream = f.InputStream;
					BinaryReader binaryReader = new BinaryReader(stream);
					this.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
					this.UpdatePrimaryImage();
				}
				return 0;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}


		// UPDATE: Primary Image = which User will only have one image so the image would be "Primary Image"
		public sbyte UpdatePrimaryImage() {
			try {
				Models.Database db = new Database();
				long NewUserID;

				if (this.UserImage.ImageID == 0) {
					NewUserID = db.InsertUserImage(this);

					if (NewUserID > 0) UserImage.ImageID = NewUserID;
				}
				else {
					db.UpdateUserImage(this);
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}



		// GET: User Information for Incident
		public List<Incident> GetIncidents(int IncidentID = 0) {
			try {
				Database db = new Database();
				Models.Incident i = new Models.Incident();

				return db.GetIncidents(IncidentID, i.OwnerUserName);
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}


		// GET: User Session
		public User GetUserSession() {
			try {
				User u = new User();
				if (HttpContext.Current.Session["CurrentUser"] == null) {
					return u;
				}
				u = (User)HttpContext.Current.Session["CurrentUser"];
				return u;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}


		// SAVE: User Session
		public bool SaveUserSession() {
			try {
				HttpContext.Current.Session["CurrentUser"] = this;
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}


		// REMOVE: User Session
		public bool RemoveUserSession() {
			try {
				HttpContext.Current.Session["CurrentUser"] = null;
				return true;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}

		// enumuration
		public enum ActionTypes {
			NoType = 0,
			InsertSuccessful = 1,
			UpdateSuccessful = 2,
			DuplicateEmail = 3,
			DuplicateUserID = 4,
			Unknown = 5,
			RequiredFieldsMissing = 6,
			LoginFailed = 7
		}

	}

}