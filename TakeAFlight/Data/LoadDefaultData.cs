using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TakeAFlight.Models;

namespace TakeAFlight.Data
{
	/// <summary>
	/// an extension class for loading the data for the first time to db.
	/// mainly from json
	/// </summary>
	public static class LoadDefaultData
	{

		public static void LoadDefaulDestinationData(TakeAFlightContext context)
		{
			var Folder = Directory.GetCurrentDirectory();
			string jsonString = File.ReadAllText(Folder + @"\bin\Debug\netcoreapp2.0\wwwroot\lib\Data\Destinastion.json");
			JArray jsonData = JArray.Parse(jsonString);

			List<Destination> destList = jsonData.ToObject<List<Destination>>();
			destList.ForEach(obj => context.Destinations.Add(obj));
			context.SaveChanges();
		}
	}
	
}
