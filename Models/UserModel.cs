using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models;

public class User
{
    public Guid ID { get; set; }
    [Required]
    [EmailAddress]
    public String Email { get; set; }
    [Required]
    public String Password { get; set; }
    public String Name { get; set; }
    public String Surname { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
}