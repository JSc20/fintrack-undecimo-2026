using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
	public class SavingsGoalController : Controller
	{
		private readonly ApplicationDbContext _db;

		public SavingsGoalController(ApplicationDbContext db)
		{
			_db = db;
		}

		// GET: SavingsGoal
		public IActionResult Index()
		{
			IEnumerable<SavingsGoal> goals = _db.SavingsGoals;
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
			// Nombre único por usuario
			bool nombreExiste = _db.SavingsGoals.Any(g => g.UserId == goal.UserId && g.GoalName == goal.GoalName);
			if (nombreExiste)
			{
				ModelState.AddModelError("GoalName", "Ya tienes una meta de ahorro con ese nombre.");
			}

			if (ModelState.IsValid)
			{
				_db.SavingsGoals.Add(goal);
				_db.SaveChanges();
				return RedirectToAction(nameof(Index));
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

			var goal = _db.SavingsGoals.Find(id);
			if (goal == null)
			{
				return NotFound();
			}

			return View(goal);
		}

		// POST: SavingsGoal/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(SavingsGoal goal)
		{
			bool nombreExiste = _db.SavingsGoals.Any(g => g.UserId == goal.UserId && g.GoalName == goal.GoalName && g.GoalId != goal.GoalId);
			if (nombreExiste)
			{
				ModelState.AddModelError("GoalName", "Ya tienes una meta de ahorro con ese nombre.");
			}

			if (ModelState.IsValid)
			{
				_db.SavingsGoals.Update(goal);
				_db.SaveChanges();
				return RedirectToAction(nameof(Index));
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

			var goal = _db.SavingsGoals.FirstOrDefault(g => g.GoalId == id);
			if (goal == null)
			{
				return NotFound();
			}

			return View(goal);
		}

		// POST: SavingsGoal/Delete
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirm(int id)
		{
			var goal = _db.SavingsGoals.Find(id);
			if (goal == null)
			{
				return NotFound();
			}

			_db.SavingsGoals.Remove(goal);
			_db.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
	}
}