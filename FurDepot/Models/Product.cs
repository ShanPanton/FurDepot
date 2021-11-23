using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FurDepot.Models
{
	public class Product
	{
		public int ProductID = 0;
		public string ProductName = string.Empty;
		public string ProductDesc = string.Empty;
		//Added these changes
		public decimal ProductCost = 0;
		public DateTime DateSold;
		public DateTime DatePost;


		//public DateTime Start; // no published date?
		//public DateTime End; 
		public User User;

		public List<Image> Images;
		public Image ProductImage;

		public ActionTypes ActionType = ActionTypes.NoType;
		public bool IsActive = true;
		public int AverageRating = 0;

		public Image PrimaryImage
		{
			get {
				if (this.Images != null) {
					foreach (Image i in this.Images) {
						if (i.Primary) return i;
					}
				}
				return new Image();
			}
		}

		//public bool Editable
		//{
		//	get {
		//		if (this.Start == null) return true;
		//		if (this.Start > DateTime.Now) return true;
		//		return false;
		//	}
		//}

		public Product GetProduct(int ProductID)
		{
			try {
				Database db = new Database();
				List<Product> products = new List<Product>();
				if (this.User == null) {
					products = db.GetProducts(ProductID);
				}
				else {
					products = db.GetProducts(ProductID, this.User.UserID);
				}
				return products[0];
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}


		public Product.ActionTypes Save()
		{
			try {
				Database db = new Database();
				if (ProductID == 0) //insert new user
				{
					this.ActionType = db.InsertProduct(this);
				}
				else {
					this.ActionType = db.UpdateProduct(this);
				}
				return this.ActionType;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}


		public sbyte AddProductImage(HttpPostedFileBase f)
		{
			try {
				this.ProductImage = new Image();
				this.ProductImage.Primary = false;
				this.ProductImage.FileName = Path.GetFileName(f.FileName);

				if (this.ProductImage.IsImageFile()) {
					this.ProductImage.ImageSize = f.ContentLength;
					Stream stream = f.InputStream;
					BinaryReader binaryReader = new BinaryReader(stream);
					this.ProductImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
					this.UpdatePrimaryImage();
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }

		}


		public sbyte UpdatePrimaryImage()
		{
			try {
				Database db = new Database();
				int NewintProductID;
				if (this.ProductImage.ImageID == 0) {
					NewintProductID = db.InsertProductImage(this);
					if (NewintProductID > 0) ProductImage.ImageID = NewintProductID;
				}
				else {
					db.UpdateProductImage(this);
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public enum ActionTypes
		{
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