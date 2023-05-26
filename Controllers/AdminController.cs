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
        return View(EditFlight.ToEditFlight(flight));
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
            bool ticketExists = await _context.Tickets.AnyAsync(t => t.FlightID == flightId && t.Seat == i);

            if (!ticketExists)
            {
                var ticket = new Ticket()
                {
                    FlightID = flightId,
                    Seat = i
                };
                await _context.Tickets.AddAsync(ticket);
            }
        }
        await _context.SaveChangesAsync();
    }
    [Authorize(Roles = "Manager, Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateFlight(CreateFlight inputFlight, string? seats)
    {
        if(ModelState.IsValid)
        {
            Flight flight = inputFlight.ToFlight();
            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(seats))
            {
                await AddSeats(seats, flight.ID);
            }
            return RedirectToAction("FlightManagement", "Admin");
        }

        return View(inputFlight);
    }
    public IActionResult UserManagement()
    {
        var users = _context.Users.Include(u => u.Role).ToList();
        var roles = _context.Roles.ToList();

        var model = new Tuple<List<User>, List<Role>>(users, roles);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(UserManagement));
    }
    [HttpPost]
    public async Task<IActionResult> ChangeUserRole(Guid userId, int roleId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            return NotFound();
        }

        user.RoleID = roleId;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(UserManagement));
    }
}