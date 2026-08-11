using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public IncomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        private IActionResult? CheckSession()
        {
            if (GetSessionUserId() == null)
                return RedirectToAction("Login", "User");
            return null;
        }

        // GET: Income
        public IActionResult Index()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var incomes = _db.Incomes
                .Include(i => i.Category)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.IncomeDate)
                .ToList();

            var mesActual = DateTime.Now;
            ViewBag.TotalIngresosMes = incomes
                .Where(i => i.IncomeDate.Month == mesActual.Month && i.IncomeDate.Year == mesActual.Year)
                .Sum(i => i.IncomeAmount);

            // Cargar categorías activas para los dropdowns de los modales
            ViewBag.Categories = _db.Categories
                .Where(c => c.CategoryStatus)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View(incomes);
        }

        // POST: Income/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Income income)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Validaciones de Reglas de Negocio
            if (!_db.Categories.Any(c => c.CategoryId == income.CategoryId))
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");

            if (income.IncomeDate > DateTime.Now)
                ModelState.AddModelError("IncomeDate", "La fecha del ingreso no puede ser posterior al día de hoy.");

            if (income.IsFixed && string.IsNullOrEmpty(income.Frequency))
                ModelState.AddModelError("Frequency", "Los ingresos fijos requieren una frecuencia.");

            if (!income.IsFixed)
                income.Frequency = null;

            if (!ModelState.IsValid)
            {
                // En lugar de retornar una vista inexistente, enviamos el error a Index
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(Index));
            }

            income.UserId = userId;
            _db.Incomes.Add(income);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Ingreso registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Income/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Income income)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Verificar pertenencia del registro al usuario en sesión
            var existingIncome = _db.Incomes.FirstOrDefault(i => i.IncomeId == income.IncomeId && i.UserId == userId);
            if (existingIncome == null) return NotFound();

            // Validaciones
            if (income.IncomeDate > DateTime.Now)
                ModelState.AddModelError("IncomeDate", "La fecha no puede ser posterior a hoy.");

            if (income.IsFixed && string.IsNullOrEmpty(income.Frequency))
                ModelState.AddModelError("Frequency", "Seleccione una frecuencia para el ingreso fijo.");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(Index));
            }

            // Actualización de campos
            existingIncome.IncomeType = income.IncomeType;
            existingIncome.IncomeAmount = income.IncomeAmount;
            existingIncome.IncomeDate = income.IncomeDate;
            existingIncome.CategoryId = income.CategoryId;
            existingIncome.IsFixed = income.IsFixed;
            existingIncome.Frequency = income.IsFixed ? income.Frequency : null;

            _db.Incomes.Update(existingIncome);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Ingreso actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Income/DeleteConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var income = _db.Incomes.FirstOrDefault(i => i.IncomeId == id && i.UserId == userId);

            if (income != null)
            {
                _db.Incomes.Remove(income);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Ingreso eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}