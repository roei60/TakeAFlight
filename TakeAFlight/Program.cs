using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TakeAFlight.Models.ML;

namespace TakeAFlight
{
	public class Program
	{
		public static void Main(string[] args)
		{
            DestinationPredictionManager.Instance.Train();
            BuildWebHost(args).Run();

        }

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();
	}
}
