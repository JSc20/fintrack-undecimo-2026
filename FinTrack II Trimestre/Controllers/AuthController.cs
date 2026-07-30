using System.Security.Claims;
using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuthController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _db.Users.FirstOrDefault(u => u.name == model.name);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(model);
        }

        if (!user.status)
        {
            ModelState.AddModelError(string.Empty, "La cuenta está desactivada.");
            return View(model);
        }

        if (user.LoginAttempts >= 3)
        {
            ModelState.AddModelError(string.Empty, "Cuenta bloqueada por demasiados intentos fallidos.");
            return View(model);
        }

        if (user.password != model.password)
        {
            user.LoginAttempts++;
            _db.SaveChanges();

            if (user.LoginAttempts >= 3)
            {
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada por demasiados intentos fallidos.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            }

            return View(model);
        }

        user.LoginAttempts = 0;
        _db.SaveChanges();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.name),
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
