using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public enum Sex
	{
		Man=0,
		Female
	};

	public class Passenger
	{
		public int PassengerID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PhoneNumber { get; set; }
		public Sex Gender { get; set; }
		public string  Nationality { get; set; }
		public DateTime? DateOfBirth { get; set; }



	}
}
