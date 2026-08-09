using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Método auxiliar: obtener UserId de la sesión activa
        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        // Redirige a login si no hay sesión activa
        private IActionResult? CheckSession()
        {
            if (GetSessionUserId() == null)
                return RedirectToAction("Login", "User");
            return null;
        }

        // GET: Profile — muestra el perfil del usuario en sesión
        public IActionResult Index()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.FirstOrDefault(p => p.UserId == userId);

            // Si no tiene perfil creado, redirigir a creación
            if (profile == null)
                return RedirectToAction(nameof(Create));

            return View(profile);
        }

        // GET: Profile/Create
        public IActionResult Create()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Un usuario solo puede tener un perfil
            if (_db.Profiles.Any(p => p.UserId == userId))
                return RedirectToAction(nameof(Index));

            return View();
        }

        // POST: Profile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Profile profile)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Prevenir duplicado de perfil
            if (_db.Profiles.Any(p => p.UserId == userId))
            {
                TempData["ErrorMessage"] = "Ya tienes un perfil creado.";
                return RedirectToAction(nameof(Index));
            }

            // Asignar el UserId de la sesión activa
            profile.UserId = userId;

            if (ModelState.IsValid)
            {
                _db.Profiles.Add(profile);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Perfil creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }

        // GET: Profile/Edit
        public IActionResult Edit()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.FirstOrDefault(p => p.UserId == userId);

            if (profile == null) return RedirectToAction(nameof(Create));
            return View(profile);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Profile profile)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Evitar que se edite el perfil de otro usuario
            profile.UserId = userId;

            if (ModelState.IsValid)
            {
                _db.Profiles.Update(profile);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }

        // GET: Profile/Delete
        public IActionResult Delete()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.FirstOrDefault(p => p.UserId == userId);

            if (profile == null) return NotFound();
            return View(profile);
        }

        // POST: Profile/DeleteConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.FirstOrDefault(p => p.ProfileId == id && p.UserId == userId);

            if (profile == null) return NotFound();

            _db.Profiles.Remove(profile);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Perfil eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
