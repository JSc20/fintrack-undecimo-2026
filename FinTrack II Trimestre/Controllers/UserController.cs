using Microsoft.AspNetCore.Mvc;
using FinTrack_II_Trimestre.Models;
using FinTrack_II_Trimestre.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FinTrack_II_Trimestre.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            // Validaciones
            if (string.IsNullOrEmpty(user.Username) || user.Username.Length < 5 || user.Username.Length > 20)
            {
                ModelState.AddModelError("Username", "El nombre de usuario debe tener entre 5 y 20 caracteres.");
            }

            if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 8 || !user.Password.Any(char.IsUpper))
            {
                ModelState.AddModelError("Password", "La contraseña debe tener al menos 8 caracteres y una mayúscula.");
            }

            if (_context.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "El nombre de usuario ya existe.");
            }

            if (!ModelState.IsValid)
            {
                return View("Register", user);
            }

            // Hash de la contraseña
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                user.Password = string.Join("", hash.Select(b => b.ToString("x2")));
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null)
            {
                ModelState.AddModelError("Username", "Usuario no encontrado.");
                return View("Login", model);
            }

            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                var inputHash = string.Join("", hash.Select(b => b.ToString("x2")));

                if (user.Password != inputHash)
                {
                    user.LoginAttempts++;
                    if (user.LoginAttempts >= 3)
                    {
                        ModelState.AddModelError("Password", "Máximo de 3 intentos fallidos.");
                        return View("Login", model);
                    }
                }
                else
                {
                    user.LoginAttempts = 0;
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Profile", "Profile");
        }
    }
}
