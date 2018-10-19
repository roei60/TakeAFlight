using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public enum Sex
	{
		Man=0,
		Female
	};
	public enum Nationality
	{
		USA=0,
		Israel,
		Europe,
		Asia
	}
	public class Passenger
	{
		public int PassengerID { get; set; }
		public int ApplicationUserID { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public Sex Gender { get; set; }
		[Required]
		public Nationality Nationality { get; set; }
		[Required]
		public DateTime? DateOfBirth { get; set; }

		public ApplicationUser User { get; set; }

	}
}
