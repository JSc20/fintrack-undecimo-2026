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

        // GET: Expense
        public IActionResult Index()
        {
            IEnumerable<Expense> expenses = _db.Expenses.Include(e => e.Category);
            return View(expenses);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == expense.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");
            }

            if (ModelState.IsValid)
            {
                _db.Expenses.Add(expense);
                _db.SaveChanges();

                // RN-08: Distribución automática de ingresos
                // Al registrar un egreso, se amortiza automáticamente con los ingresos registrados
                // TODO: Implementar cuando el módulo BudgetPlan esté disponible

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName", expense.CategoryId);
            return View(expense);
        }

        // GET: Expense/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expense = _db.Expenses.Find(id);
            if (expense == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName", expense.CategoryId);
            return View(expense);
        }

        // POST: Expense/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Expense expense)
        {
            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == expense.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");
            }

            if (ModelState.IsValid)
            {
                _db.Expenses.Update(expense);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName", expense.CategoryId);
            return View(expense);
        }

        // GET: Expense/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expense = _db.Expenses.Include(e => e.Category).FirstOrDefault(e => e.ExpenseId == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expense/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var expense = _db.Expenses.Find(id);
            if (expense == null)
            {
                return NotFound();
            }

            _db.Expenses.Remove(expense);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
