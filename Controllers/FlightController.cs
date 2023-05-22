using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CourseWork.Models;
using CourseWork.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CourseWork.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseWork.Controllers;

public class FlightController : Controller
{
    private readonly ApplicationDbContext _context;
    public FlightController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult List()
    {
        var flights = _context.Flights.Where(f => f.DepartureDate > DateTime.Now).ToList();
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
        flight.Tickets = _context.Tickets.Where(t => t.FlightID == FlightId && t.BookedBy == null).ToList();

        string errorMessage = TempData["ErrorMessage"] as string;
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ModelState.AddModelError(string.Empty, errorMessage);
        }
        return View(flight);
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Payment(string id, List<int> selectedSeats)
    {
        if(selectedSeats.Count != 0 && selectedSeats != null)
        {
            Flight flight = await _context.Flights
            .Include(f => f.Tickets)
            .SingleOrDefaultAsync(f => f.ID == Guid.Parse(id));
            FlightViewModel model = new(flight, selectedSeats);
            if (flight.DepartureDate < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Бронювання квитків на вже початий рейс недоступне";
                return RedirectToAction("Details", new { FlightId = Guid.Parse(id) });
            }
            foreach (Ticket ticket in flight.GetSelectedTickets(selectedSeats))
            {
            if(ticket.BookedBy != null)
                {
                    TempData["ErrorMessage"] = "Деякі з обраних квитків вже були заброньовані";
                    return RedirectToAction("Details", new { FlightId = Guid.Parse(id) });
                }
            }
            return View(model);
        }
        TempData["ErrorMessage"] = "Оберіть хоча-б одне місце";
        return RedirectToAction("Details", new { FlightId = Guid.Parse(id) });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> BookTickets(string id, List<int> selectedSeats)
    {
        if(User.Identity.IsAuthenticated)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value);
            Flight flight = await _context.Flights
            .Include(f => f.Tickets)
            .SingleOrDefaultAsync(f => f.ID == Guid.Parse(id));

            foreach (Ticket ticket in flight.GetSelectedTickets(selectedSeats))
            {
                ticket.BookedBy = userId;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Tickets", "Account");
        }
        return View(Details(Guid.Parse(id)));
    }
    [Authorize]
    public async Task SetTicketOwnerNull(int seat, Guid flightId)
    {
        Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value);
        Ticket ticket = await _context.Tickets
        .SingleOrDefaultAsync(t => t.Seat == seat && t.FlightID == flightId && t.BookedBy == userId);
        ticket.BookedBy = null;
        _context.SaveChanges();
    }
    [Authorize]
    public async Task<IActionResult> UnbookTicket(int seat, Guid flightId)
    {
        await SetTicketOwnerNull(seat, flightId);
        return RedirectToAction("Tickets", "Account");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}