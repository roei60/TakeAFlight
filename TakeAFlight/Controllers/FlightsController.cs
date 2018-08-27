using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TakeAFlight.Data;
using TakeAFlight.Models;

namespace TakeAFlight.Controllers
{
	/// <summary>
	/// flight controller will be used for search flight, 
	/// add,update and remove in admin mode
	/// and etc... mainly use flight model
	/// </summary>
    public class FlightsController : Controller
    {
        private readonly TakeAFlightContext _context;
		
        public FlightsController(TakeAFlightContext context)
        {
			_context = context;
		}

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            var takeAFlightContext = _context.Flight.Include(f => f.Destination);
            return View(await takeAFlightContext.ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight
                .Include(f => f.Destination)
                .SingleOrDefaultAsync(m => m.FlightID == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
			ViewBag.Items = _context.Destinations.Select(obj => new SelectListItem()
			{
				Text = obj.ToString(),
				Value = obj.DestinationID.ToString()
			}).ToList();


			return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlightID,DestinationID,Price,Duration,Departure")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DestinationID"] = new SelectList(_context.Set<Destination>(), "DestinationID", "DestinationID", flight.DestinationID);
            return View(flight);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight.SingleOrDefaultAsync(m => m.FlightID == id);
            if (flight == null)
            {
                return NotFound();
            }
			//     ViewData["DestinationID"] = new SelectList(_context.Set<Destination>(), "DestinationID", "DestinationID", flight.DestinationID);
			ViewBag.Items = _context.Destinations.Select(obj => new SelectListItem()
			{
				Text = obj.ToString(),
				Value = obj.DestinationID.ToString()
			}).ToList();
			return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlightID,DestinationID,Price,Duration,Departure")] Flight flight)
        {
            if (id != flight.FlightID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.FlightID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DestinationID"] = new SelectList(_context.Set<Destination>(), "DestinationID", "DestinationID", flight.DestinationID);
            return View(flight);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight
                .Include(f => f.Destination)
                .SingleOrDefaultAsync(m => m.FlightID == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flight.SingleOrDefaultAsync(m => m.FlightID == id);
            _context.Flight.Remove(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flight.Any(e => e.FlightID == id);
        }
    }
}
