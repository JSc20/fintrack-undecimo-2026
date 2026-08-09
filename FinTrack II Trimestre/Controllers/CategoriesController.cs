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

		// GET: Category
		public IActionResult Index()
		{
			int userId = GetCurrentUserId();
			IEnumerable<Category> categories = _db.Categories.Where(c => c.UserId == userId);
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
			int userId = GetCurrentUserId();
			category.UserId = userId;

			bool nombreExiste = _db.Categories.Any(c => c.UserId == userId && c.CategoryName == category.CategoryName);
			if (nombreExiste)
			{
				ModelState.AddModelError("CategoryName", "Ya existe una categoría con ese nombre.");
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

		// GET: Category/Edit/5
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

		// POST: Category/Edit
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

			bool nombreExiste = _db.Categories.Any(c => c.UserId == userId && c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId);
			if (nombreExiste)
			{
				ModelState.AddModelError("CategoryName", "Ya existe una categoría con ese nombre.");
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

		// GET: Category/Delete/5
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

		// POST: Category/Delete
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