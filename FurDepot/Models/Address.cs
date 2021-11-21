using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurDepot.Models
{
	public class Address
	{
		public string strAddress = string.Empty;
		public string strCity = string.Empty;
		public string strState = string.Empty;
		public string strZipCode = string.Empty;

		public string FullAddress //if there is no set, that means it is read only (get only)
		{
			get {
				if (this.strAddress.Length == 0 || this.strCity.Length == 0 || this.strState.Length == 0 || this.strZipCode.Length == 0) {
					return string.Empty;
				}
				else {
					return string.Concat(this.strAddress, ", ", this.strCity, ", ", this.strState, ", ", this.strZipCode);
				}
			}
		}
	}
}