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

        // Método auxiliar: obtener UserId de sesión (null si no hay sesión activa)
        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        // Redirige a login si no hay sesión activa
        private IActionResult? CheckSession()
        {
            if (GetSessionUserId() == null)
                return RedirectToAction("Login", "User");
            return null;
        }

        // Método auxiliar: Validar que las categorías sumen 100%
        private IActionResult? CheckBudget100Percent(int userId)
        {
            decimal totalPercentage = _db.Categories.Where(c => c.UserId == userId).Sum(c => c.CategoryPercentage);
            if (totalPercentage != 100.00m)
            {
                TempData["Error"] = "Debes configurar tus categorías hasta alcanzar exactamente el 100% de tu presupuesto antes de registrar transacciones.";
                return RedirectToAction("Index", "Category");
            }
            return null;
        }

        // GET: Income — lista ingresos
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

            // RF-03: Calcular total de ingresos del mes actual para el dashboard
            var mesActual = DateTime.Now;
            var totalIngresosMes = incomes
                .Where(i => i.IncomeDate.Month == mesActual.Month && i.IncomeDate.Year == mesActual.Year)
                .Sum(i => i.IncomeAmount);

            ViewBag.TotalIngresosMes = totalIngresosMes;

            // Datos financieros adicionales para las stat cards
            var expenses = _db.Expenses.Where(e => e.UserId == userId).ToList();
            ViewBag.TotalEgresosMes = expenses
                .Where(e => e.ExpenseDate.Month == mesActual.Month && e.ExpenseDate.Year == mesActual.Year)
                .Sum(e => e.ExpenseAmount);
            var savingsGoals = _db.SavingsGoals.Where(s => s.UserId == userId).ToList();
            ViewBag.TotalAhorro = savingsGoals.Sum(s => s.CurrentAmount);

            return View(incomes);
        }

        // GET: Income/Create
        public IActionResult Create()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Bloqueo si las categorías no suman 100%
            var budgetCheck = CheckBudget100Percent(userId);
            if (budgetCheck != null) return budgetCheck;

            ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus && c.UserId == userId), "CategoryId", "CategoryName");
            ViewBag.Frequencies = new SelectList(new[] { "Mensual", "Quincenal", "Semanal" });
            return View();
        }

        // POST: Income/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Income income)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Bloqueo si las categorías no suman 100%
            var budgetCheck = CheckBudget100Percent(userId);
            if (budgetCheck != null) return budgetCheck;

            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == income.CategoryId);
            if (!categoryExists)
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");

            // RF-02: Validar que la fecha no sea posterior al día de hoy
            if (income.IncomeDate > DateTime.Now)
                ModelState.AddModelError("IncomeDate", "La fecha del ingreso no puede ser posterior al día de hoy.");

            // RF-04: Si es fijo, la frecuencia es obligatoria
            if (income.IsFixed && string.IsNullOrEmpty(income.Frequency))
                ModelState.AddModelError("Frequency", "Los ingresos fijos requieren una frecuencia (Mensual, Quincenal o Semanal).");

            // RF-04: Si es variable, no debe tener frecuencia
            if (!income.IsFixed)
                income.Frequency = null;

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus && c.UserId == userId),
                    "CategoryId", "CategoryName", income.CategoryId);
                ViewBag.Frequencies = new SelectList(new[] { "Mensual", "Quincenal", "Semanal" }, income.Frequency);
                return View(income);
            }

            // Asignar el usuario
            income.UserId = userId;

            _db.Incomes.Add(income);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Ingreso registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Income/Edit/5
        public IActionResult Edit(int? id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            if (id == null || id == 0) return NotFound();

            int userId = GetSessionUserId()!.Value;

            // Bloqueo si las categorías no suman 100%
            var budgetCheck = CheckBudget100Percent(userId);
            if (budgetCheck != null) return budgetCheck;

            var income = _db.Incomes.FirstOrDefault(i => i.IncomeId == id && i.UserId == userId);
            if (income == null) return NotFound();

            ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus && c.UserId == userId),
                "CategoryId", "CategoryName", income.CategoryId);
            ViewBag.Frequencies = new SelectList(new[] { "Mensual", "Quincenal", "Semanal" }, income.Frequency);
            return View(income);
        }

        // POST: Income/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Income income)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Bloqueo si las categorías no suman 100%
            var budgetCheck = CheckBudget100Percent(userId);
            if (budgetCheck != null) return budgetCheck;

            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == income.CategoryId);
            if (!categoryExists)
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");

            // RF-02: Validar fecha no futura
            if (income.IncomeDate > DateTime.Now)
                ModelState.AddModelError("IncomeDate", "La fecha del ingreso no puede ser posterior al día de hoy.");

            // RF-04: Si es fijo, frecuencia obligatoria
            if (income.IsFixed && string.IsNullOrEmpty(income.Frequency))
                ModelState.AddModelError("Frequency", "Los ingresos fijos requieren una frecuencia.");

            if (!income.IsFixed)
                income.Frequency = null;

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus && c.UserId == userId),
                    "CategoryId", "CategoryName", income.CategoryId);
                ViewBag.Frequencies = new SelectList(new[] { "Mensual", "Quincenal", "Semanal" }, income.Frequency);
                return View(income);
            }

            // Mantener el UserId de la sesión para evitar suplantación
            income.UserId = userId;

            _db.Incomes.Update(income);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Ingreso actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Income/Delete/5
        public IActionResult Delete(int? id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            if (id == null || id == 0) return NotFound();

            int userId = GetSessionUserId()!.Value;
            var income = _db.Incomes
                .Include(i => i.Category)
                .FirstOrDefault(i => i.IncomeId == id && i.UserId == userId);

            if (income == null) return NotFound();
            return View(income);
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

            if (income == null) return NotFound();

            _db.Incomes.Remove(income);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Ingreso eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}