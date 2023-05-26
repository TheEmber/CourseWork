using Microsoft.AspNetCore.Mvc;
using CourseWork.Models;
using CourseWork.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CourseWork.ViewModels;

namespace CourseWork.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
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
                user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == user.Email);

                await Authenticate(user);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Обліковий запис з вказаною електронною поштою вже існує");
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
                return RedirectToAction("Details", "Account");
            }
            ModelState.AddModelError("", "Невірно введені електронна пошта або пароль");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        return View(model);
    }

    private async Task Authenticate(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.ID.ToString()),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
        };
        ClaimsIdentity id = new(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }

    [Authorize]
    public async Task<IActionResult> Details()
    {
        string? userId = HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;
        UserProfileViewModel model = new()
        {
            User = await _context.Users
            .Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.ID == Guid.Parse(userId))
        };
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(UserProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            string userId = HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;

            User user = await _context.Users
                .SingleOrDefaultAsync(u => u.ID == Guid.Parse(userId));
            if (user != null && user.VerifyHashedPassword(model.OldPassword))
            {
                user.ChangePassword(model.OldPassword, model.NewPassword);
                await _context.SaveChangesAsync();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Logout");
            }
        }
        return View();
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
    [Authorize]
    public IActionResult Tickets()
    {
        Guid userId = Guid.Parse(HttpContext!.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value);
        var flights = _context.Flights
        .Where(f => f.Tickets.Any(t => t.BookedBy == userId))
        .ToList();
        List<FlightViewModel> model = new();
        foreach (var flight in flights)
        {
            var tickets = _context.Tickets
            .Where(t => t.BookedBy == userId && t.FlightID == flight.ID)
            .Select(t => t.Seat)
            .ToList();
            FlightViewModel mod = new(flight, tickets);
            model.Add(mod);
        }
        return View(model);
    }
}