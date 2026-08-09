using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
	public class SavingsGoalController : BaseController
	{
		private readonly ApplicationDbContext _db;

		public SavingsGoalController(ApplicationDbContext db)
		{
			_db = db;
		}

		// GET: SavingsGoal
		public IActionResult Index()
		{
			int userId = GetCurrentUserId();
			IEnumerable<SavingsGoal> goals = _db.SavingsGoals.Where(g => g.UserId == userId);
			return View(goals);
		}

		// GET: SavingsGoal/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: SavingsGoal/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(SavingsGoal goal)
		{
			int userId = GetCurrentUserId();
			goal.UserId = userId;

			bool nombreExiste = _db.SavingsGoals.Any(g => g.UserId == userId && g.GoalName == goal.GoalName);
			if (nombreExiste)
			{
				ModelState.AddModelError("GoalName", "Ya tienes una meta de ahorro con ese nombre.");
			}

			if (goal.CurrentAmount > goal.TargetAmount)
			{
				ModelState.AddModelError("CurrentAmount", "El monto actual no puede ser mayor al monto objetivo.");
			}

			if (ModelState.IsValid)
			{
				try
				{
					_db.SavingsGoals.Add(goal);
					_db.SaveChanges();
					TempData["Success"] = "Meta de ahorro creada correctamente.";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception)
				{
					ModelState.AddModelError("", "Ocurrió un error al guardar la meta. Intenta de nuevo.");
				}
			}

			return View(goal);
		}

		// GET: SavingsGoal/Edit/5
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			int userId = GetCurrentUserId();
			var goal = _db.SavingsGoals.FirstOrDefault(g => g.GoalId == id);
			if (goal == null)
			{
				return NotFound();
			}

			if (goal.UserId != userId)
			{
				return Forbid();
			}

			return View(goal);
		}

		// POST: SavingsGoal/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(SavingsGoal goal)
		{
			int userId = GetCurrentUserId();
			var existing = _db.SavingsGoals.AsNoTracking().FirstOrDefault(g => g.GoalId == goal.GoalId);
			if (existing == null)
			{
				return NotFound();
			}

			if (existing.UserId != userId)
			{
				return Forbid();
			}

			goal.UserId = userId;

			bool nombreExiste = _db.SavingsGoals.Any(g => g.UserId == userId && g.GoalName == goal.GoalName && g.GoalId != goal.GoalId);
			if (nombreExiste)
			{
				ModelState.AddModelError("GoalName", "Ya tienes una meta de ahorro con ese nombre.");
			}

			if (goal.CurrentAmount > goal.TargetAmount)
			{
				ModelState.AddModelError("CurrentAmount", "El monto actual no puede ser mayor al monto objetivo.");
			}

			if (ModelState.IsValid)
			{
				try
				{
					_db.SavingsGoals.Update(goal);
					_db.SaveChanges();
					TempData["Success"] = "Meta de ahorro actualizada correctamente.";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception)
				{
					ModelState.AddModelError("", "Ocurrió un error al actualizar la meta. Intenta de nuevo.");
				}
			}

			return View(goal);
		}

		// GET: SavingsGoal/Delete/5
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			int userId = GetCurrentUserId();
			var goal = _db.SavingsGoals.FirstOrDefault(g => g.GoalId == id);
			if (goal == null)
			{
				return NotFound();
			}

			if (goal.UserId != userId)
			{
				return Forbid();
			}

			return View(goal);
		}

		// POST: SavingsGoal/Delete
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirm(int id)
		{
			int userId = GetCurrentUserId();
			var goal = _db.SavingsGoals.FirstOrDefault(g => g.GoalId == id);
			if (goal == null)
			{
				return NotFound();
			}

			if (goal.UserId != userId)
			{
				return Forbid();
			}

			try
			{
				_db.SavingsGoals.Remove(goal);
				_db.SaveChanges();
				TempData["Success"] = "Meta de ahorro eliminada correctamente.";
			}
			catch (Exception)
			{
				TempData["Error"] = "Ocurrió un error al eliminar la meta.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}