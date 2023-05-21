using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CourseWork.Models;
using CourseWork.Data;
namespace CourseWork.Controllers;

public class FlightController : Controller
{
    private readonly ApplicationDbContext _context;
    public FlightController(ApplicationDbContext context)
    {
        _context = context;
    }

    public ActionResult List()
    {
        var flights = _context.Flights.ToList();
        var flightsWithCount = new List<FlightWithCount>();
        foreach (var flight in flights)
        {
            int ticketCount = _context.Tickets.Count(t => t.FlightID == flight.ID && t.BookedBy == null);
            var flightWithCount = flight.ToFlightWithCount(ticketCount);
            flightsWithCount.Add(flightWithCount);
        }

        return View(flightsWithCount);
    }

    public IActionResult Details(Guid FlightId)
    {
        Flight flight = _context.Flights.FirstOrDefault(f => f.ID == FlightId);
        var tickets = _context.Tickets.Where(t => t.FlightID == FlightId).ToList();

        return View(flight);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}