using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public class FlightOrder
	{
		public int FlightOrderID { get; set; }
		public int FlightID { get; set; }

		public int PassengerID { get; set; }

        public int Quantity { get; set; }
	
		public Passenger Passenger { get; set; }
		public Flight Flight { get; set; }

	}
}
