using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CourseWork.Models;
using Microsoft.AspNetCore.Authorization;
using CourseWork.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CourseWork.ViewModels;

namespace CourseWork.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public AdminController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    [Authorize(Roles = "Manager, Admin")]
    public IActionResult FlightManagement()
    {
        var flights = _context.Flights.AsParallel().ToList();
        var flightsWithCount = new List<FlightWithCount>();
        foreach (var flight in flights)
        {
            int ticketCount = _context.Tickets.Count(t => t.FlightID == flight.ID && t.BookedBy == null);
            var flightWithCount = flight.ToFlightWithCount(ticketCount);
            flightsWithCount.Add(flightWithCount);
        }

        return View(flightsWithCount);
    }
    [Authorize(Roles = "Manager, Admin")]
    public IActionResult CreateFlight()
    {
        var flight = new CreateFlight();

        return View(flight);
    }
    [Authorize(Roles = "Manager, Admin")]
    public IActionResult DeleteFlight(Guid flightId)
    {
        Flight flight = _context.Flights.Find(flightId);
        _context.Flights.Remove(flight);
        _context.SaveChangesAsync();
        return RedirectToAction("FlightManagement", "Admin");
    }
    [Authorize(Roles = "Manager, Admin")]
    public IActionResult FlightDetails(Guid flightId)
    {
        Flight flight = _context.Flights.FirstOrDefault(f => f.ID == flightId);
        flight.Tickets = _context.Tickets.Where(t => t.FlightID == flightId && t.BookedBy == null).ToList();

        string errorMessage = TempData["ErrorMessage"] as string;
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ModelState.AddModelError(string.Empty, errorMessage);
        }
        return View(flight);
    }
    [HttpPost]
    [Authorize(Roles = "Manager, Admin")]
    public IActionResult UpdateFlight(EditFlight model, string? seats)
    {
        if (ModelState.IsValid)
        {
            Flight flight = _context.Flights.Find(model.ID);

            if (flight != null)
            {
                flight.Source = model.Source;
                flight.Destination = model.Destination;
                flight.DepartureDate = model.DepartureDate;
                flight.ArrivalDate = model.ArrivalDate;
                flight.Price = model.Price;

                _context.SaveChanges();
                if (!string.IsNullOrEmpty(seats))
                {
                    AddSeats(seats, flight.ID);
                }

                return RedirectToAction("FlightManagement", "Admin");
            }
        }
        return View("FlightDetails", model);
    }
    [Authorize(Roles = "Manager, Admin")]
    public async Task AddSeats(string seatsRange, Guid flightId)
    {
        var seats = seatsRange.Split('-').Select(int.Parse).ToList();
        for(int i = seats[0]; i<= seats[1]; i++)
        {
            // Check if a ticket with the same FlightID and Seat already exists
            bool ticketExists = _context.Tickets.Any(t => t.FlightID == flightId && t.Seat == i);

            if (!ticketExists)
            {
                var ticket = new Ticket()
                {
                    FlightID = flightId,
                    Seat = i
                };
                _context.Tickets.Add(ticket);
            }
        }
        await _context.SaveChangesAsync();
    }
    [Authorize(Roles = "Manager, Admin")]
    [HttpPost]
    public IActionResult CreateFlight(CreateFlight inputFlight, string? seats)
    {
        if(ModelState.IsValid)
        {
            Flight flight = inputFlight.ToFlight();
            _context.Flights.Add(flight);
            _context.SaveChanges();
            if (!string.IsNullOrEmpty(seats))
            {
                AddSeats(seats, flight.ID);
            }
        }

        return View(inputFlight);
    }
}