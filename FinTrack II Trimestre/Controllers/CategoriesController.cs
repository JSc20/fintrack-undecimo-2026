using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _db;

		public CategoryController(ApplicationDbContext db)
		{
			_db = db;
		}

		// GET: Category
		public IActionResult Index()
		{
			IEnumerable<Category> categories = _db.Categories;
			return View(categories);
		}

		// GET: Category/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Category/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Category category)
		{
			// RN-01: Nombre de categoría único
			bool nombreExiste = _db.Categories.Any(c => c.CategoryName == category.CategoryName);
			if (nombreExiste)
			{
				ModelState.AddModelError("CategoryName", "Ya existe una categoría con ese nombre.");
			}

			if (ModelState.IsValid)
			{
				_db.Categories.Add(category);
				_db.SaveChanges();
				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		// GET: Category/Edit/5
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _db.Categories.Find(id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		// POST: Category/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Category category)
		{
			// RN-01: Nombre de categoría único (excluyendo el registro actual)
			bool nombreExiste = _db.Categories.Any(c => c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId);
			if (nombreExiste)
			{
				ModelState.AddModelError("CategoryName", "Ya existe una categoría con ese nombre.");
			}

			if (ModelState.IsValid)
			{
				_db.Categories.Update(category);
				_db.SaveChanges();
				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		// GET: Category/Delete/5
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _db.Categories.FirstOrDefault(c => c.CategoryId == id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		// POST: Category/Delete
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirm(int id)
		{
			var category = _db.Categories.Find(id);
			if (category == null)
			{
				return NotFound();
			}

			// Evitar borrar categorías que ya tienen gastos asociados
			bool tieneGastos = _db.Expenses.Any(e => e.CategoryId == id);
			if (tieneGastos)
			{
				TempData["Error"] = "No se puede eliminar la categoría porque tiene gastos asociados.";
				return RedirectToAction(nameof(Index));
			}

			_db.Categories.Remove(category);
			_db.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
	}
}