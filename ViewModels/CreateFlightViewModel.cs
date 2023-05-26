namespace CourseWork.Models;

using System.ComponentModel.DataAnnotations;
public class CreateFlight
{
    [Required(ErrorMessage = "Введіть місце вильоту")]
    public string Source { get; set; }
    [Required(ErrorMessage = "Введіть місце прибуття")]
    public string Destination { get; set; }
    [Required(ErrorMessage = "Введіть коректну дату вильоту")]
    public DateTime DepartureDate { get; set; }
    [Required(ErrorMessage = "Введіть коректну дату прибуття")]
    public DateTime ArrivalDate { get; set; }
    [Required(ErrorMessage = "Введіть коректну ціну за квиток")]
    public double Price { get; set; }
    public Flight ToFlight()
    {
        return new Flight()
        {
            ID = Guid.NewGuid(),
            Source = this.Source,
            Destination = this.Destination,
            DepartureDate = this.DepartureDate,
            ArrivalDate = this.ArrivalDate,
            Price = this.Price
        };
    }
}