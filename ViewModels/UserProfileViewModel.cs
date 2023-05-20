using System.ComponentModel.DataAnnotations;
using CourseWork.Models;

namespace CourseWork;

public class UserProfileViewModel
{
    public User? User { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? OldPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Паролі не співпадають.")]
    public string? ConfirmNewPassword { get; set; }
}