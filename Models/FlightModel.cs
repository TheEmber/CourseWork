using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models;
public class Flight {
    [Required]
    public Guid ID { get; set; }
    [Required]
    public string Source { get; set; }
    [Required]
    public string Destination { get; set; }
    [Required]
    public DateTime DepartureDate { get; set; }
    [Required]
    public DateTime ArrivalDate { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public ICollection<Ticket>? Tickets { get; set; }
    public FlightWithCount ToFlightWithCount(int count)
    {
        return new FlightWithCount
        {
            ID = this.ID,
            Source = this.Source,
            Destination = this.Destination,
            DepartureDate = this.DepartureDate,
            ArrivalDate = this.ArrivalDate,
            Price = this.Price,
            TicketCount = count
        };
    }
    public ICollection<Ticket> GetSelectedTickets(List<int> selectedSeats)
    {
        return Tickets.Where(t => selectedSeats.Contains(t.Seat)).ToList();
    }
}

public class FlightWithCount : Flight
{
    public int TicketCount { get; set; }
}