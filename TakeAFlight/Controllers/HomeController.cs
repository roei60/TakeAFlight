using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TakeAFlight.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TakeAFlight.Controllers
{
    public class HomeController : Controller
    {
        TakeAFlightContext dbContext;
        public HomeController(TakeAFlightContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            ViewBag.Items = dbContext.Destinations.Select(obj => new SelectListItem()
            {
                Text = obj.ToString(),
                Value = obj.DestinationID.ToString()
            }).ToList();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}