using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace TakeAFlight.Models
{
	public class TakeAFlightContext : DbContext
	{
		public TakeAFlightContext(DbContextOptions<TakeAFlightContext> options)
			: base(options)
		{


		}
		public DbSet<Passenger> Passengers { get; set; }
		public DbSet<TakeAFlight.Models.Destination> Destinations { get; set; }
		public DbSet<TakeAFlight.Models.Flight> Flight { get; set; }


	}
}