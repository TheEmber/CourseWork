using System.ComponentModel.DataAnnotations;
using CourseWork.Models;

namespace CourseWork.ViewModels;

public class FlightViewModel : Flight
{
    [Required(ErrorMessage = "Оберіть квиток")]
    public List<int> Seats { get; set; }
    public double TotalPrice { get; set; }
    public FlightViewModel(Flight flight, List<int> seats)
    {
        ID = flight.ID;
        Source = flight.Source;
        Destination = flight.Destination;
        DepartureDate = flight.DepartureDate;
        ArrivalDate = flight.ArrivalDate;
        Price = flight.Price;
        Seats = seats;
        TotalPrice = Price * Seats.Count;
    }
}