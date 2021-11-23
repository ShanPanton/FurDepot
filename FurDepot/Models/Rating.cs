using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurDepot.Models
{
	public class Rating
	{
		public int RateID = 0;
		public int UserID = 0;
		public int ProductID = 0;
		public byte intRating = 0;
		public Types Type = Types.NoType;

		public enum Types
		{
			NoType = 0,
			Product = 1,
			User = 2,
			Image = 3
		}
	}
}