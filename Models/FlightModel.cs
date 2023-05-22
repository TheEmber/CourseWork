using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models;
public class Flight {
    public Guid ID { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public double Price { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
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