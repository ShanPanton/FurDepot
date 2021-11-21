using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurDepot.Models {
	public class ProductContent {
		public Product Product;
		public User User;

		public bool CurrentUserIsOwner {

			get {
				if (Product == null) return false;
				if (Product.User == null) return false;
				if (User == null) return false;
				if (User.UserID != Product.User.UserID) return false;
				return true;
			}
		}

	}
}