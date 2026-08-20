using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Categories
        public IActionResult Index()
        {
            int userId = GetCurrentUserId();
            IEnumerable<Category> categories = _db.Categories.Where(c => c.UserId == userId).ToList();

            // Calcular el total de porcentajes actuales para mostrarlo en la vista
            decimal totalPercentage = categories.Sum(c => c.CategoryPercentage);
            ViewBag.TotalPorcentaje = totalPercentage;

            // Estadísticas financieras para las stat cards
            var mesActual = DateTime.Now;
            var incomes = _db.Incomes.Where(i => i.UserId == userId).ToList();
            var expenses = _db.Expenses.Where(e => e.UserId == userId).ToList();
            var savingsGoals = _db.SavingsGoals.Where(s => s.UserId == userId).ToList();

            ViewBag.TotalIngresosMes = incomes
                .Where(i => i.IncomeDate.Month == mesActual.Month && i.IncomeDate.Year == mesActual.Year)
                .Sum(i => i.IncomeAmount);
            ViewBag.TotalEgresosMes = expenses
                .Where(e => e.ExpenseDate.Month == mesActual.Month && e.ExpenseDate.Year == mesActual.Year)
                .Sum(e => e.ExpenseAmount);
            ViewBag.TotalAhorro = savingsGoals.Sum(s => s.CurrentAmount);
            ViewBag.IngresosExtra = incomes.Where(i => !i.IsFixed).Sum(i => i.IncomeAmount);

            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            int userId = GetCurrentUserId();
            category.UserId = userId;

            // Validación 1: Nombre duplicado
            bool nombreExiste = _db.Categories.Any(c => c.UserId == userId && c.CategoryName.ToLower() == category.CategoryName.ToLower());
            if (nombreExiste)
            {
                ModelState.AddModelError("CategoryName", "Ya existe una categoría con ese nombre.");
            }

            // Validación 2: Límite del 100%
            decimal currentTotal = _db.Categories.Where(c => c.UserId == userId).Sum(c => c.CategoryPercentage);
            if (currentTotal + category.CategoryPercentage > 100.00m)
            {
                decimal disponible = 100.00m - currentTotal;
                ModelState.AddModelError("CategoryPercentage", $"No puedes superar el 100% del presupuesto. Actualmente tienes {disponible}% disponible.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Categories.Add(category);
                    _db.SaveChanges();
                    TempData["Success"] = "Categoría creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar la categoría. Intenta de nuevo.");
                }
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            int userId = GetCurrentUserId();
            var category = _db.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.UserId != userId)
            {
                return Forbid();
            }

            return View(category);
        }

        // POST: Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            int userId = GetCurrentUserId();
            var existing = _db.Categories.AsNoTracking().FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existing == null)
            {
                return NotFound();
            }

            if (existing.UserId != userId)
            {
                return Forbid();
            }

            category.UserId = userId;

            // Validación 1: Nombre duplicado (excluyendo la actual)
            bool nombreExiste = _db.Categories.Any(c => c.UserId == userId && c.CategoryName.ToLower() == category.CategoryName.ToLower() && c.CategoryId != category.CategoryId);
            if (nombreExiste)
            {
                ModelState.AddModelError("CategoryName", "Ya existe una categoría con ese nombre.");
            }

            // Validación 2: Límite del 100% (sumamos todas MENOS la que estamos editando, y le sumamos el nuevo valor propuesto)
            decimal currentTotalExcludingThis = _db.Categories
                .Where(c => c.UserId == userId && c.CategoryId != category.CategoryId)
                .Sum(c => c.CategoryPercentage);

            if (currentTotalExcludingThis + category.CategoryPercentage > 100.00m)
            {
                decimal disponible = 100.00m - currentTotalExcludingThis;
                ModelState.AddModelError("CategoryPercentage", $"El ajuste supera el 100% del presupuesto. Solo puedes asignarle hasta un {disponible}%.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Categories.Update(category);
                    _db.SaveChanges();
                    TempData["Success"] = "Categoría actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error al actualizar la categoría. Intenta de nuevo.");
                }
            }

            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            int userId = GetCurrentUserId();
            var category = _db.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.UserId != userId)
            {
                return Forbid();
            }

            return View(category);
        }

        // POST: Categories/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            int userId = GetCurrentUserId();
            var category = _db.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.UserId != userId)
            {
                return Forbid();
            }

            bool tieneGastos = _db.Expenses.Any(e => e.CategoryId == id);
            if (tieneGastos)
            {
                TempData["Error"] = "No se puede eliminar la categoría porque tiene gastos asociados.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
                TempData["Success"] = "Categoría eliminada correctamente.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al eliminar la categoría.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}