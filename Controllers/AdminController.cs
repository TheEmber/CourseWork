using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CourseWork.Models;

namespace CourseWork.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public AdminController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Add()
    {
        return View();
    }
}