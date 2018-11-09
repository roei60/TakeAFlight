using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public class Flight
	{
		public int FlightID { get; set; }
		[Required]
		public int DestinationID { get; set; }
		[Required]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Price Should be a postive number")]
		public double Price { get; set; }
		[Required]
		public TimeSpan? Duration { get; set; }
		[Required]
		public DateTime? Departure { get; set; }

		public virtual Destination Destination { get; set; }
	}
}
