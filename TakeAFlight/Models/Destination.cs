using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public class Destination
	{
		[DisplayName("Destination")]
		public int  DestinationID { get; set; }
		[Required]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Country should contain letters only")]
		public string Country { get; set; }
		[Required]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "City should contain letters only")]
		public string City { get; set; }

		public override string ToString()
		{
			return Country + "," + City;
		}
	}
}
