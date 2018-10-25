using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models
{
	public enum Sex
	{
		Man = 0,
		Female
	};
	public enum Nationality
	{
		USA = 0,
		Israel,
		Europe,
		Asia
	}
	public class Passenger
	{
		public int ID { get; set; }
		[Required]
		[Range(100000000, 999999999, ErrorMessage = "Passenger ID should be 9 numbers")]
		[Display (Name ="Passenger ID")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Passenger ID Should be a number")]
		public int IdPassenger{ get; set; }
		public string  ApplicationUserID { get; set; }
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name should conatins letters only")]
		public string FirstName { get; set; }
		[Required]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name should conatins letters only")]
		public string LastName { get; set; }
		[Required]
		public Sex Gender { get; set; }
		[Required]
		public Nationality Nationality { get; set; }
		[Required]
		[Display(Name ="Birth Date")]
		public DateTime? DateOfBirth { get; set; }

		public ApplicationUser User { get; set; }

	}
}
