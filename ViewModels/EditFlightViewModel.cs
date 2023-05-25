using CourseWork.Models;

namespace CourseWork.ViewModels;

public class EditFlight : CreateFlight
{
    public Guid ID { get; set; }
    public Flight ToFlight()
    {
        return new Flight()
        {
            ID = this.ID,
            Source = this.Source,
            Destination = this.Destination,
            DepartureDate = this.DepartureDate,
            ArrivalDate = this.ArrivalDate,
            Price = this.Price
        };
    }
}