using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers;

public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;

    public UsersController(ApplicationDbContext db)
    {
        _db = db;
    }

    [Authorize]
    public IActionResult Index()
    {
        var users = _db.Users.ToList();
        return View(users);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(User user)
    {
        if (ModelState.IsValid)
        {
            var exists = _db.Users.Any(u => u.name == user.name);
            if (exists)
            {
                ModelState.AddModelError("name", "El nombre de usuario ya está en uso.");
                return View(user);
            }

            user.status = true;
            user.LoginAttempts = 0;
            _db.Users.Add(user);
            _db.SaveChanges();

            return RedirectToAction("Login", "Auth");
        }
        return View(user);
    }
}
