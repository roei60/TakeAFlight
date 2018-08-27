using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public class Destination
	{
		[DisplayName("Destination")]
		public int  DestinationID { get; set; }
		public string Country { get; set; }
		public string City { get; set; }

		public override string ToString()
		{
			return Country + "," + City;
		}
	}
}
