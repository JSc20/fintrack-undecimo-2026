using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ExpenseController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Método auxiliar: obtener UserId de sesión
        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        // Redirige a login si no hay sesión activa
        private IActionResult? CheckSession()
        {
            if (GetSessionUserId() == null)
                return RedirectToAction("Login", "User");
            return null;
        }

        // GET: Expense — lista gastos del usuario en sesión
        public IActionResult Index()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var expenses = _db.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();

            // RF-05: Calcular total de gastos del mes actual
            var mesActual = DateTime.Now;
            var totalGastosMes = expenses
                .Where(e => e.ExpenseDate.Month == mesActual.Month && e.ExpenseDate.Year == mesActual.Year)
                .Sum(e => e.ExpenseAmount);

            ViewBag.TotalGastosMes = totalGastosMes;

            return View(expenses);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == expense.CategoryId);
            if (!categoryExists)
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");

            // RF-02: Validar que la fecha no sea futura
            if (expense.ExpenseDate > DateTime.Now)
                ModelState.AddModelError("ExpenseDate", "La fecha del gasto no puede ser posterior al día de hoy.");

            // RF-07: Si es fijo, la fecha de vencimiento es obligatoria
            if (expense.IsFixed && !expense.DueDate.HasValue)
                ModelState.AddModelError("DueDate", "Los gastos fijos requieren una fecha de vencimiento.");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus),
                    "CategoryId", "CategoryName", expense.CategoryId);
                return View(expense);
            }

            // Asignar el usuario de la sesión
            expense.UserId = userId;

            _db.Expenses.Add(expense);
            _db.SaveChanges();

            // RN-08: Placeholder — distribución automática pendiente de BudgetPlan
            TempData["SuccessMessage"] = "Gasto registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Edit/5
        public IActionResult Edit(int? id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            if (id == null || id == 0) return NotFound();

            int userId = GetSessionUserId()!.Value;
            var expense = _db.Expenses.FirstOrDefault(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();

            ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus),
                "CategoryId", "CategoryName", expense.CategoryId);
            return View(expense);
        }

        // POST: Expense/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Expense expense)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == expense.CategoryId);
            if (!categoryExists)
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");

            // RF-02: Validar fecha no futura
            if (expense.ExpenseDate > DateTime.Now)
                ModelState.AddModelError("ExpenseDate", "La fecha del gasto no puede ser posterior al día de hoy.");

            // RF-07: Si es fijo, la fecha de vencimiento es obligatoria
            if (expense.IsFixed && !expense.DueDate.HasValue)
                ModelState.AddModelError("DueDate", "Los gastos fijos requieren una fecha de vencimiento.");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus),
                    "CategoryId", "CategoryName", expense.CategoryId);
                return View(expense);
            }

            // Mantener el UserId de la sesión para evitar suplantación
            expense.UserId = userId;

            _db.Expenses.Update(expense);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Gasto actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Delete/5
        public IActionResult Delete(int? id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            if (id == null || id == 0) return NotFound();

            int userId = GetSessionUserId()!.Value;
            var expense = _db.Expenses
                .Include(e => e.Category)
                .FirstOrDefault(e => e.ExpenseId == id && e.UserId == userId);

            if (expense == null) return NotFound();
            return View(expense);
        }

        // POST: Expense/DeleteConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var expense = _db.Expenses.FirstOrDefault(e => e.ExpenseId == id && e.UserId == userId);

            if (expense == null) return NotFound();

            _db.Expenses.Remove(expense);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Gasto eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
