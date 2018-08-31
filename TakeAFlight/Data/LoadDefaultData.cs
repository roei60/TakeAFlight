
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

		//Creating Admin Role to handle admin options like edit and delete a flight for example.
		public static  async void CreateRolesandUsers(RoleManager<IdentityRole> roleManager,
			UserManager<ApplicationUser> userManager)
		{

			bool isRoleExist = await roleManager.RoleExistsAsync("Admin");
			if (!isRoleExist)
			{

				// first we create Admin rool    
				var role = new IdentityRole();
				role.Name = "Admin";
				await roleManager.CreateAsync(role);

				//Here we create a Admin super user who will maintain the website                   

				var user = new ApplicationUser();
				user.UserName = "Admin@gmail.com";
				user.Email = "Admin@gmail.com";

				string userPWD = "Admin1234!";

				IdentityResult chkUser = await userManager.CreateAsync(user, userPWD);

				//Add default User to Role Admin    
				if (chkUser.Succeeded)
				{
					var result1 = await userManager.AddToRoleAsync(user, "Admin");
				}
			}

		}
		public static  void CreateRandomFlightsData(TakeAFlightContext context)
		{
			Random rnd = new Random();
			List<Destination> destinationsList = context.Destinations.ToList();
			for (int i = 0; i < 200; i++)
			{
				int randomIndex = rnd.Next(0, destinationsList.Count);
				Destination selectedDest = destinationsList[randomIndex];
				//									year,  month,       day,  hour,     min,      sec
			 	DateTime deparuteData = new DateTime(2019, i % 12+1, i % 28+1, i % 23+1, i % 12 * 5, 0);
				TimeSpan durationTime = new TimeSpan(i % 5+1, i % 12 * 5, 0);
				double price = rnd.Next(500, 2000);
				Flight tempFlight = new Flight()
				{
					DestinationID = selectedDest.DestinationID,
					Departure = deparuteData,
					Duration = durationTime,
					Price = price,
					Destination = selectedDest
				};
				 context.Flight.Add(tempFlight);

			}
			 context.SaveChanges();
		}
	}

}
