using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TakeAFlight.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        [HttpPost]
        public JsonResult GetDestinations()
        {
            var Dest = dbContext.Destinations;
            return new JsonResult(Dest);

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

        [HttpGet]
        public JsonResult CountFlightsByYear(int Year = 2019)
        {
           
            var qr = from selection in dbContext.Flight
                     where selection.Departure.Value.Year == Year
                     group selection by selection.Departure.Value.Month into grp
                     select new { key= grp.Key, value = grp.Count() };

            return Json(qr.ToList());



        }
        [HttpGet]
        public JsonResult AvgPriceByYear(int year=2019)
        {
          
            var qr = from selection in dbContext.Flight
                     where selection.Departure.Value.Year == year
                     group selection by selection.Departure.Value.Month into grp
                     select new { key = grp.Key, value = Math.Round(grp.Average(obj => obj.Price)) };



            return Json(qr.ToList());





        }





    }
}