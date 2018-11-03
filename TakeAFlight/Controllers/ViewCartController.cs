using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(List<int> data)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Show()
        {
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Show(List<string> FlightId)
        {
            FlightId.RemoveAt(0);
            var takeAFlightContext = _context.Flight.Include(f => f.Destination);
            var Flights = from flights in takeAFlightContext
                          where FlightId.Contains(flights.FlightID.ToString())
                          select flights;
            int pageSize = 100;
            var model = await PagingList.CreateAsync(Flights, pageSize, 1, "FlightID", "FlightID");
            return View(model);
        }
    }
}
