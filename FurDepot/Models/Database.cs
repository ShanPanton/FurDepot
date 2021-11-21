using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace FurDepot.Models {

	public class Database {


		// INSERT: User
		public User.ActionTypes InsertUser(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Insert_TUsers", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@userid", u.UserID, SqlDbType.Int, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@username", u.UserName, SqlDbType.VarChar);
				SetParameter(ref cm, "@password", u.Password, SqlDbType.VarChar);
				SetParameter(ref cm, "@firstname", u.FirstName, SqlDbType.VarChar);
				SetParameter(ref cm, "@lastname", u.LastName, SqlDbType.VarChar);
				SetParameter(ref cm, "@email", u.Email, SqlDbType.VarChar);
				SetParameter(ref cm, "@phone", u.Phone, SqlDbType.VarChar);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: // new user created
						u.UserID = (int)cm.Parameters["@userid"].Value;
						return User.ActionTypes.InsertSuccessful;
					case -1:
						return User.ActionTypes.DuplicateEmail;
					case -2:
						return User.ActionTypes.DuplicateUserID;
					default:
						return User.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}




		// UPDATE: User
		public User.ActionTypes UpdateUser(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Update_TUsers", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@userid", u.UserID, SqlDbType.Int);
				SetParameter(ref cm, "@username", u.UserName, SqlDbType.VarChar);
				SetParameter(ref cm, "@password", u.Password, SqlDbType.VarChar);
				SetParameter(ref cm, "@firstname", u.FirstName, SqlDbType.VarChar);
				SetParameter(ref cm, "@lastname", u.LastName, SqlDbType.VarChar);
				SetParameter(ref cm, "@email", u.Email, SqlDbType.VarChar);
				SetParameter(ref cm, "@phone", u.Phone, SqlDbType.VarChar);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: //new updated
						return User.ActionTypes.UpdateSuccessful;
					default:
						return User.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}




		// LOGIN
		public User Login(User u) {
			try {
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Login", cn);
				DataSet ds;
				User newUser = null;

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				SetParameter(ref da, "@username", u.UserName, SqlDbType.VarChar);
				SetParameter(ref da, "@password", u.Password, SqlDbType.VarChar);

				try {
					ds = new DataSet();
					da.Fill(ds);
					if (ds.Tables[0].Rows.Count > 0) {
						newUser = new User();
						DataRow dr = ds.Tables[0].Rows[0];
						newUser.UserID = (int)dr["UserID"];
						newUser.UserName = u.UserName;
						newUser.Password = u.Password;
						newUser.FirstName = (string)dr["FirstName"];
						newUser.LastName = (string)dr["LastName"];
						newUser.Email = (string)dr["Email"];
						newUser.Phone = (string)dr["Phone"];
					}
				}
				catch (Exception ex) {
					throw new Exception(ex.Message);
				}
				finally {
					CloseDBConnection(ref cn);
				}
				return newUser; //alls well in the world
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		// GET: User Image
		public List<Image> GetUserImage(long UserID = 0, long UserImageID = 0, bool PrimaryOnly = true) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Select_TUser_Image", cn);
				List<Image> imgs = new List<Image>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (UserID > 0) SetParameter(ref da, "@userid", UserID, SqlDbType.Int);
				if (UserImageID > 0) SetParameter(ref da, "@userimageid", UserImageID, SqlDbType.BigInt);
				if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					throw new Exception(ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Image i = new Image();
						i.ImageID = (long)dr["UserImageID"];
						i.ImageData = (byte[])dr["Image"];
						i.FileName = (string)dr["FileName"];
						i.ImageSize = (long)dr["ImageSize"];
						if (dr["PrimaryImage"].ToString() == "Y")
							i.Primary = true;
						else
							i.Primary = false;
						imgs.Add(i);
					}
				}
				return imgs;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}



		// INSERT: User Image
		public long InsertUserImage(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Insert_TUser_Image", cn);

				SetParameter(ref cm, "@userimageid", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@userid", u.UserID, SqlDbType.Int);

				if (u.UserImage.Primary)
					SetParameter(ref cm, "@primaryimage", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primaryimage", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@filename", u.UserImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@imagesize", u.UserImage.ImageSize, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);
				return (long)cm.Parameters["@userimageid"].Value;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}




		// UPDATE: User Image
		public long UpdateUserImage(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Update_TUser_Image", cn);

				SetParameter(ref cm, "@userimageid", u.UserImage.ImageID, SqlDbType.BigInt);

				if (u.UserImage.Primary)
					SetParameter(ref cm, "@primaryimage", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primaryimage", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@filename", u.UserImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@imagesize", u.UserImage.ImageSize, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);

				return 0; //success	
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}




		// DELETE: User Image
		public bool DeleteUserImage(long UserImageID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Delete_TUser_Image", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@userimageid", UserImageID, SqlDbType.BigInt);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;

			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		/*-----------------------------------------------------*/
		// GET: Product
		public List<Product> GetProducts(int ProductID = 0, int UserID = 0) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Select_TProducts", cn);
				List<Product> products = new List<Product>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (ProductID > 0) SetParameter(ref da, "@intproductid", ProductID, SqlDbType.Int);
				if (UserID > 0) SetParameter(ref da, "@intuserid", UserID, SqlDbType.Int);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally {
					CloseDBConnection(ref cn);
				}

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Product p = new Product();
						p.ProductID = (int)dr["ProductID"];
						p.ProductName = (string)dr["ProductName"];
						p.ProductDesc = (string)dr["ProductDesc"];
						p.ProductCost = (decimal)dr["ProductCost"];
						
						if (dr["PostedDate"] != null) p.DatePost = (DateTime)dr["PostedDate"];
						if (dr["SoldDate"] != null) p.DateSold = (DateTime)dr["SoldDate"];

						/*if (dr["StartDate"] != null) p.Start = (DateTime)dr["StartDate"];*/ // product published date
																							  //if (dr["EndDate"] != null) p.End = (DateTime)dr["EndDate"]; // product sold date?

						if (dr["IsActive"].ToString() == "N") p.IsActive = false;

						p.User = new User();
						p.User.UserID = (int)dr["UserID"];
						p.User.UserName = (string)dr["Username"];
						p.User.FirstName = (string)dr["FirstName"];
						p.User.LastName = (string)dr["LastName"];
						p.User.Email = (string)dr["Email"];
						

						List<Image> images = GetImagesForProducts(p.ProductID, 0, true);
						if (images.Count > 0) p.ProductImage = images[0];

						products.Add(p);
					}
				}
				return products;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		//Added changes to the insert of the product 
		// INSERT: Product
		public Product.ActionTypes InsertProduct(Product p) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_TPRODUCTS", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@intproductid", p.ProductID, SqlDbType.Int, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@intuserid", p.User.UserID, SqlDbType.Int);
				SetParameter(ref cm, "@strproduct", p.ProductName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@strdescription", p.ProductDesc, SqlDbType.NVarChar);

				SetParameter(ref cm, "@monproductcost", p.ProductCost, SqlDbType.Money);
				SetParameter(ref cm, "@dtPostedDate", p.DatePost, SqlDbType.DateTime);
				SetParameter(ref cm, "@dtSoldDate", p.DateSold, SqlDbType.DateTime);

				/*SetParameter(ref cm, "@start_date", p.Start, SqlDbType.DateTime);*/  // published date?
																					   /*SetParameter(ref cm, "@end_date", p.End, SqlDbType.DateTime);*/  // sold date?

				if (p.IsActive)
					SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: // new product created
						p.ProductID = (int)cm.Parameters["@intproductid"].Value;
						return Product.ActionTypes.InsertSuccessful;
					default:
						return Product.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}




		// UPDATE: Product
		public Product.ActionTypes UpdateProduct(Product p) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Update_TProducts", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@intproductid", p.ProductID, SqlDbType.Int);
				SetParameter(ref cm, "@intuserid", p.User.UserID, SqlDbType.Int);
				SetParameter(ref cm, "@strproduct", p.ProductName, SqlDbType.VarChar);
				SetParameter(ref cm, "@strdescription", p.ProductDesc, SqlDbType.VarChar);


				SetParameter(ref cm, "@monproductcost", p.ProductCost, SqlDbType.Money);
				SetParameter(ref cm, "@dtPostedDate", p.DatePost, SqlDbType.DateTime);
				SetParameter(ref cm, "@dtSoldDate", p.DateSold, SqlDbType.DateTime);

				/*SetParameter(ref cm, "@start", p.Start, SqlDbType.DateTime);*/ // published date
																				 /*SetParameter(ref cm, "@end", p.End, SqlDbType.DateTime);*/ // sold date?

				if (p.IsActive)
					SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: //new updated
						return Product.ActionTypes.UpdateSuccessful;
					default:
						return Product.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		// DELETE: Product
		public bool DeleteProduct(int ProductID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Delete_TProducts", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@intproductid", ProductID, SqlDbType.Int);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		// GET: Product Ratings
		public List<Rating> GetProductRatings(int UserID) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Select_TUsers_Product_Ratings", cn);
				List<Rating> ratings = new List<Rating>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				SetParameter(ref da, "@userid", UserID, SqlDbType.Int);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally {
					CloseDBConnection(ref cn);
				}

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Rating r = new Rating();
						r.Type = Rating.Types.Product;
						r.ProductID = (int)dr["ProductID"];

						// need to check if r.Rate changed to r.intRating, where is the for each loop?
						r.intRating = (byte)dr["Rating"];
						ratings.Add(r);
					}
				}
				return ratings;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		//UPDATE: Product Ratings
		public int RateProduct(int UserID, int ProductID, int intRating) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Update_TProduct_Ratings", cn);
				int intReturnValue = -1;

				// check database
				SetParameter(ref cm, "@rateid", null, SqlDbType.Int, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@userid", UserID, SqlDbType.Int);
				SetParameter(ref cm, "@productid", ProductID, SqlDbType.Int);
				SetParameter(ref cm, "@rating", intRating, SqlDbType.TinyInt);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				//1 = new rate added
				//2 = existing rate updated
				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);
				return intReturnValue;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}

	
		public List<Product> GetActiveProducts() {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Select_TProducts_Active", cn);
				List<Product> products = new List<Product>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					throw new Exception(ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Product p = new Product();
						p.ProductID = (int)dr["ProductID"];
						p.ProductName = (string)dr["ProductName"];
						p.ProductDesc = (string)dr["ProductDesc"];
						p.ProductCost = (decimal)dr["ProductCost"];
						//if (dr["StartDate"] != null) p.Start = (DateTime)dr["StartDate"]; // product published date?
						//if (dr["EndDate"] != null) p.End = (DateTime)dr["EndDate"]; // product sold date?

						if (dr["IsActive"].ToString() == "N") p.IsActive = false;

						p.User = new User();
						p.User.UserID = (int)dr["UserID"];
						p.User.FirstName = (string)dr["FirstName"];
						p.User.LastName = (string)dr["LastName"];

						List<Image> images = GetImagesForProducts(p.ProductID, 0, true);
						if (images.Count > 0) p.ProductImage = images[0];

						products.Add(p);
					}
				}
				return products;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}
		public Product GetProductDetails(int productId) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Select_TProducts_Details", cn);
				da.SelectCommand.Parameters.Add("@intinputproductid", SqlDbType.Int).Value = productId;
				Product product = new Product();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					throw new Exception(ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					DataRow dr = ds.Tables[0].Rows[0];
					product.ProductID = (int)dr["ProductID"];
					product.ProductName = (string)dr["ProductName"];
					product.ProductDesc = (string)dr["ProductDesc"];
					product.ProductCost = (decimal)dr["ProductCost"];
					//if (dr["StartDate"] != null) p.Start = (DateTime)dr["StartDate"]; // product published date?
					//if (dr["EndDate"] != null) p.End = (DateTime)dr["EndDate"]; // product sold date?

					if (dr["IsActive"].ToString() == "N") product.IsActive = false;

					product.User = new User();
					product.User.UserID = (int)dr["UserID"];
					product.User.FirstName = (string)dr["FirstName"];
					product.User.LastName = (string)dr["LastName"];

					List<Image> images = GetImagesForProducts(product.ProductID, 0, true);
					if (images.Count > 0) product.ProductImage = images[0];
				}
				return product;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		// GET: Product Images
		public List<Image> GetImagesForProducts(int ProductID = 0, long ProductImageID = 0, bool PrimaryOnly = false) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Select_TProduct_Images", cn);
				List<Image> imgs = new List<Image>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (ProductID > 0) SetParameter(ref da, "@productid", ProductID, SqlDbType.Int);
				if (ProductImageID > 0) SetParameter(ref da, "@productimageid", ProductImageID, SqlDbType.BigInt);
				if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally {
					CloseDBConnection(ref cn);
				}

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Image i = new Image();
						i.ImageID = (int)dr["ProductImageID"];
						i.ImageData = (byte[])dr["Image"];
						i.FileName = (string)dr["FileName"];
						i.ImageSize = (int)dr["ImageSize"];
						if (dr["PrimaryImage"].ToString() == "Y")
							i.Primary = true;
						else
							i.Primary = false;
						imgs.Add(i);
					}
				}
				return imgs;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}


		// INSERT: Product Image
		public long InsertProductImage(Product p) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Insert_TProduct_Images", cn);

				SetParameter(ref cm, "@productimageid", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@productid", p.ProductID, SqlDbType.Int);
				if (p.ProductImage.Primary)
					SetParameter(ref cm, "@primaryimage", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primaryimage", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", p.ProductImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@filename", p.ProductImage.FileName, SqlDbType.VarChar);
				SetParameter(ref cm, "@imagesize", p.ProductImage.ImageSize, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);
				return (long)cm.Parameters["@productimageid"].Value;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}


		// UPDATE: Product Image
		public long UpdateProductImage(Product p) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Update_TProduct_Images", cn);

				SetParameter(ref cm, "@productimageid", p.ProductImage.ImageID, SqlDbType.Int);
				if (p.ProductImage.Primary)
					SetParameter(ref cm, "@primaryimage", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primaryimage", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", p.ProductImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@filename", p.ProductImage.FileName, SqlDbType.VarChar);
				SetParameter(ref cm, "@imagesize", p.ProductImage.ImageSize, SqlDbType.Int);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);

				return 0; //success	
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}
		
	


		// DELETE: Product Image
		public bool DeleteProductImage(int ProductID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Delete_TProduct_Images", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@productid", ProductID, SqlDbType.Int);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}





		// GET: Incident
		public List<Incident> GetIncidents(int IncidentID = 0, string OwnerUserName = null) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("Select_TIncidents", cn);
				List<Incident> incident = new List<Incident>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (OwnerUserName != null) SetParameter(ref da, "@ownerusername", OwnerUserName, SqlDbType.VarChar);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Incident i = new Incident();
						i.IncidentID = (int)dr["IncidentID"];
						i.OwnerUserName = (string)dr["UserName"];
						i.FirstName = (string)dr["First Name"];
						i.LastName = (string)dr["Last Name"];
						i.IncidentTitle = (string)dr["Incident Title"];
						i.IncidentDesc = (string)dr["Incident Description"];
						i.UserName = (string)dr["UserName"];


						i.User = new User();
						i.User.UserName = (string)dr["UserName"];
						i.User.FirstName = (string)dr["FirstName"];
						i.User.LastName = (string)dr["LastName"];
						i.User.Email = (string)dr["Email"];

						incident.Add(i);
					}

				}
				return incident;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}



		// INSERT: Incident
		public Incident.ActionTypes InsertIncident(Incident i) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("Insert_TIncidents", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@incidentid", i.IncidentID, SqlDbType.Int, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@ownerusername", i.OwnerUserName, SqlDbType.VarChar);
				SetParameter(ref cm, "@firstname", i.FirstName, SqlDbType.VarChar);
				SetParameter(ref cm, "@lastname", i.LastName, SqlDbType.VarChar);
				SetParameter(ref cm, "@incidenttitle", i.IncidentTitle, SqlDbType.VarChar);
				SetParameter(ref cm, "@incidentdesc", i.IncidentDesc, SqlDbType.NVarChar);
				SetParameter(ref cm, "@username", i.UserName, SqlDbType.VarChar);
				//SetParameter(ref cm, "@submitteddate", i.SubmittedDate, SqlDbType.DateTime);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);


				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: // new incident created
						i.IncidentID = (int)cm.Parameters["@incidentid"].Value;
						return Incident.ActionTypes.InsertSuccessful;
					default:
						return Incident.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}


		// GET: SQL Connection
		private bool GetDBConnection(ref SqlConnection SQLConn) {
			try {
				if (SQLConn == null) SQLConn = new SqlConnection();
				if (SQLConn.State != ConnectionState.Open) {
					SQLConn.ConnectionString = ConfigurationManager.AppSettings["AppDBConnect"];
					SQLConn.Open();
				}
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}



		// CLOSE: SQL Connection
		private bool CloseDBConnection(ref SqlConnection SQLConn) {
			try {
				if (SQLConn.State != ConnectionState.Closed) {
					SQLConn.Close();
					SQLConn.Dispose();
					SQLConn = null;
				}
				return true;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}



		// SET: Parameter - Command Type where directing Stored Procedure
		private int SetParameter(ref SqlCommand cm, string ParameterName, Object Value
			, SqlDbType ParameterType, int FieldSize = -1
			, ParameterDirection Direction = ParameterDirection.Input
			, Byte Precision = 0, Byte Scale = 0) {
			try {
				cm.CommandType = CommandType.StoredProcedure;
				if (FieldSize == -1)
					cm.Parameters.Add(ParameterName, ParameterType);
				else
					cm.Parameters.Add(ParameterName, ParameterType, FieldSize);

				if (Precision > 0) cm.Parameters[cm.Parameters.Count - 1].Precision = Precision;
				if (Scale > 0) cm.Parameters[cm.Parameters.Count - 1].Scale = Scale;

				cm.Parameters[cm.Parameters.Count - 1].Value = Value;
				cm.Parameters[cm.Parameters.Count - 1].Direction = Direction;

				return 0;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}


		// SET: Parameter - Select Command where directing Stored Procedure
		private int SetParameter(ref SqlDataAdapter cm, string ParameterName, Object Value
			, SqlDbType ParameterType, int FieldSize = -1
			, ParameterDirection Direction = ParameterDirection.Input
			, Byte Precision = 0, Byte Scale = 0) {
			try {
				cm.SelectCommand.CommandType = CommandType.StoredProcedure;
				if (FieldSize == -1)
					cm.SelectCommand.Parameters.Add(ParameterName, ParameterType);
				else
					cm.SelectCommand.Parameters.Add(ParameterName, ParameterType, FieldSize);

				if (Precision > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Precision = Precision;
				if (Scale > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Scale = Scale;

				cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Value = Value;
				cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Direction = Direction;

				return 0;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}
	}
}