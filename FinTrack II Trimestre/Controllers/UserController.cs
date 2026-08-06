using Microsoft.AspNetCore.Mvc;
using FinTrack_II_Trimestre.Models;
using FinTrack_II_Trimestre.Data;
using System.Security.Cryptography;
using System.Text;

namespace FinTrack_II_Trimestre.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        // RNF-01: Tiempo de bloqueo tras 3 intentos fallidos
        private const int MaxLoginAttempts = 3;
        private const int LockoutMinutes = 15;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── REGISTRO ────────────────────────────────────────────────────────

        // GET: User/Register
        public IActionResult Register()
        {
            // Si ya hay sesión activa, redirigir al home
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            // Validar username duplicado
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "El nombre de usuario ya está en uso.");
            }

            if (!ModelState.IsValid)
                return View(user);

            // RNF-01: Hash SHA256 de la contraseña
            user.Password = HashPassword(user.Password);
            user.Status = true;
            user.LoginAttempts = 0;

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cuenta creada exitosamente. Inicia sesión.";
            return RedirectToAction(nameof(Login));
        }

        // ─── INICIO DE SESIÓN ────────────────────────────────────────────────

        // GET: User/Login
        public IActionResult Login()
        {
            // Si ya hay sesión activa, redirigir al home
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: User/Login — RF-01
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

            // Usuario no existe
            if (user == null)
            {
                ModelState.AddModelError("Username", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            // RNF-01: Verificar si la cuenta está bloqueada
            if (user.LockoutEndDate.HasValue && user.LockoutEndDate.Value > DateTime.Now)
            {
                var minutosRestantes = (int)Math.Ceiling((user.LockoutEndDate.Value - DateTime.Now).TotalMinutes);
                ModelState.AddModelError(string.Empty,
                    $"Cuenta bloqueada por {minutosRestantes} minuto(s) debido a intentos fallidos.");
                return View(model);
            }

            // RNF-01: Verificar contraseña con hash SHA256
            var inputHash = HashPassword(model.Password);

            if (user.Password != inputHash)
            {
                user.LoginAttempts++;

                // Bloquear cuenta tras MaxLoginAttempts intentos
                if (user.LoginAttempts >= MaxLoginAttempts)
                {
                    user.LockoutEndDate = DateTime.Now.AddMinutes(LockoutMinutes);
                    user.Status = false;
                    _context.SaveChanges();

                    ModelState.AddModelError(string.Empty,
                        $"Cuenta bloqueada por {LockoutMinutes} minutos tras {MaxLoginAttempts} intentos fallidos.");
                    return View(model);
                }

                _context.SaveChanges();

                var intentosRestantes = MaxLoginAttempts - user.LoginAttempts;
                ModelState.AddModelError("Password",
                    $"Contraseña incorrecta. Intentos restantes: {intentosRestantes}.");
                return View(model);
            }

            // Login exitoso: reiniciar contadores y abrir sesión
            user.LoginAttempts = 0;
            user.LockoutEndDate = null;
            user.Status = true;
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Home");
        }

        // ─── CIERRE DE SESIÓN ────────────────────────────────────────────────

        // GET: User/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // ─── HELPER PRIVADO ──────────────────────────────────────────────────

        // RNF-01: Hash SHA256 reutilizable
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return string.Join("", hash.Select(b => b.ToString("x2")));
        }
    }
}
