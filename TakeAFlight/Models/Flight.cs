using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public class Flight
	{
		public int FlightID { get; set; }
		public int DestinationID { get; set; }
		public double Price { get; set; }
		public TimeSpan? Duration { get; set; }
		public TimeSpan Departure { get; set; }

		public virtual Destination Destination { get; set; }
	}
}
