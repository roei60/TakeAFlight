using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using TakeAFlight.Data;
using TakeAFlight.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TakeAFlight.Controllers
{
    public class ViewCartController : Controller
    {
        private readonly TakeAFlightContext _context;
        private readonly ApplicationDbContext _Userscontext;

        public ViewCartController(TakeAFlightContext context, ApplicationDbContext userContext)
        {
            _context = context;
            _Userscontext = userContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {

			return RedirectToAction("Error", "Error");
		}

		[Authorize]
        [HttpPost]
        public IActionResult Index(List<int> data)
        {
			return RedirectToAction("Error", "Error");
		}


		[Authorize]
        [HttpPost]
        public async Task<IActionResult> Show(List<string> FlightId)
        {
            FlightId.RemoveAt(0);
			if(FlightId==null|| FlightId.Count()==0)
				return RedirectToAction("Error", "Error");
			var takeAFlightContext = _context.Flight.Include(f => f.Destination);
            var Flights = from flights in takeAFlightContext
                          where FlightId.Contains(flights.FlightID.ToString())
                          select flights;
            int pageSize = 10;
            var model = await PagingList.CreateAsync(Flights, pageSize, 1, "FlightID", "FlightID");
            SetRegisterListData();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order(int[] flight_id, int[] quantity, string[] dest_details, string[] departure)
        {
            var tupleList = new List<Tuple<string, string, int>> { };

            for (int i = 0; i < flight_id.Length; i++)
            {
                if (!FlightExists(flight_id[i]))
                {
                    ViewData["Message"] = tupleList;
                    return View();
                }
            }

            var user = _Userscontext.Users.FirstOrDefault(obj => obj.UserName == User.Identity.Name);
            var passanger = _context.Passengers.FirstOrDefault(obj => obj.ApplicationUserID == user.Id);

            for (int i = 0; i < flight_id.Length; i++)
            {
                if (quantity[i] <= 0) continue;

                FlightOrder fo = new FlightOrder
                {
                    FlightID = flight_id[i],
                    PassengerID = passanger.ID,
                    Quantity = quantity[i]
                };

                _context.FlightOrders.Add(fo);

                tupleList.Add(Tuple.Create(dest_details[i], departure[i], quantity[i]));
            }

            await _context.SaveChangesAsync();

            ViewData["Message"] = tupleList;
            return View();
        }

        private void SetRegisterListData()
        {
            Sex gender = new Sex();
            Nationality nationality = new Nationality();
            ViewBag.NationalityList = nationality.ToSelectList();
            ViewBag.SexList = gender.ToSelectList();

        }

        private bool FlightExists(int id)
        {
            return _context.Flight.Any(e => e.FlightID == id);
        }
    }
}
