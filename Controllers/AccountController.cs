using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CourseWork.Models;
using CourseWork.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CourseWork.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Signup(RegisterModel model)
    {
        if(ModelState.IsValid)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                user = Models.User.RegisterUser(model.Email, model.Password, model.Name, model.Surname);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await Authenticate(user);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Логін або пароль введені некорректно");
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    public async Task<IActionResult> Login(LoginModel model)
    {
        if(ModelState.IsValid)
        {
            User user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == model.Email);
            if (user != null && user.VerifyHashedPassword(model.Password))
            {
                await Authenticate(user);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Невірні логін або пароль");
        }
        return View();
    }

    private async Task Authenticate(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
        };
        ClaimsIdentity id = new(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}