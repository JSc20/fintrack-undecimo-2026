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

        // GET: Income
        public IActionResult Index()
        {
            IEnumerable<Income> incomes = _db.Incomes.Include(i => i.Category);
            return View(incomes);
        }

        // GET: Income/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Income/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Income income)
        {
            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == income.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");
            }

            // Validar que la fecha no sea posterior al día de hoy
            if (income.IncomeDate > DateTime.Now)
            {
                ModelState.AddModelError("IncomeDate", "La fecha del ingreso no puede ser posterior al día de hoy.");
            }

            if (ModelState.IsValid)
            {
                _db.Incomes.Add(income);
                _db.SaveChanges();

                // RN-07: Recalcular montos por categoría si hay un plan activo
                // TODO: Implementar cuando el módulo BudgetPlan esté disponible

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName", income.CategoryId);
            return View(income);
        }

        // GET: Income/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var income = _db.Incomes.Find(id);
            if (income == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName", income.CategoryId);
            return View(income);
        }

        // POST: Income/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Income income)
        {
            // RN-02: Validar existencia de la categoría
            var categoryExists = _db.Categories.Any(c => c.CategoryId == income.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryId", "La categoría seleccionada no existe.");
            }

            // Validar que la fecha no sea posterior al día de hoy
            if (income.IncomeDate > DateTime.Now)
            {
                ModelState.AddModelError("IncomeDate", "La fecha del ingreso no puede ser posterior al día de hoy.");
            }

            if (ModelState.IsValid)
            {
                _db.Incomes.Update(income);
                _db.SaveChanges();

                // RN-07: Recalcular montos por categoría según el plan activo
                // TODO: Implementar cuando el módulo BudgetPlan esté disponible

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "CategoryName", income.CategoryId);
            return View(income);
        }

        // GET: Income/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var income = _db.Incomes.Include(i => i.Category).FirstOrDefault(i => i.IncomeId == id);
            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }

        // POST: Income/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var income = _db.Incomes.Find(id);
            if (income == null)
            {
                return NotFound();
            }

            _db.Incomes.Remove(income);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
