using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public String Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public String Password { get; set; }
}

public class RegisterMode
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролі не співпадають.")]
    public string ConfirmPassword { get; set; }
}