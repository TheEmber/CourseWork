using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Електронну пошту не введено")]
    [EmailAddress(ErrorMessage = "Електронної пошти не існує")]
    public String Email { get; set; }
    [Required(ErrorMessage = "Пароль не введено")]
    [DataType(DataType.Password)]
    public String Password { get; set; }
}

public class RegisterModel
{
    [Required(ErrorMessage = "Електронну пошту не введено")]
    [EmailAddress(ErrorMessage = "Електронної пошти не існує")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Пароль не введено")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Новий пароль не підтверджено")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; }
    [Required(ErrorMessage = "Ім'я не введено")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Прізвище не введено")]
    public string Surname { get; set; }
}