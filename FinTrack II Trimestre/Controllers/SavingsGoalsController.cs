
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinTrack_II_Trimestre.Models;
using FinTrack_II_Trimestre.Data;

namespace FinTrack_II_Trimestre.Controllers
{
    public class SavingsGoalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavingsGoalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SAVINGSGOALS
        public async Task<IActionResult> Index()
        {
            return View(await _context.SavingsGoals.ToListAsync());
        }

        // GET: SAVINGSGOALS/Details/5
        public async Task<IActionResult> Details(int? goalid)
        {
            if (goalid == null)
            {
                return NotFound();
            }

            var savingsgoal = await _context.SavingsGoals
                .FirstOrDefaultAsync(m => m.GoalId == goalid);
            if (savingsgoal == null)
            {
                return NotFound();
            }

            return View(savingsgoal);
        }

        // GET: SAVINGSGOALS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SAVINGSGOALS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GoalId,UserId,GoalName,TargetAmount,CurrentAmount,TargetDate,Status")] SavingsGoal savingsgoal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(savingsgoal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(savingsgoal);
        }

        // GET: SAVINGSGOALS/Edit/5
        public async Task<IActionResult> Edit(int? goalid)
        {
            if (goalid == null)
            {
                return NotFound();
            }

            var savingsgoal = await _context.SavingsGoals.FindAsync(goalid);
            if (savingsgoal == null)
            {
                return NotFound();
            }
            return View(savingsgoal);
        }

        // POST: SAVINGSGOALS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? goalid, [Bind("GoalId,UserId,GoalName,TargetAmount,CurrentAmount,TargetDate,Status")] SavingsGoal savingsgoal)
        {
            if (goalid != savingsgoal.GoalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savingsgoal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingsGoalExists(savingsgoal.GoalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(savingsgoal);
        }

        // GET: SAVINGSGOALS/Delete/5
        public async Task<IActionResult> Delete(int? goalid)
        {
            if (goalid == null)
            {
                return NotFound();
            }

            var savingsgoal = await _context.SavingsGoals
                .FirstOrDefaultAsync(m => m.GoalId == goalid);
            if (savingsgoal == null)
            {
                return NotFound();
            }

            return View(savingsgoal);
        }

        // POST: SAVINGSGOALS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? goalid)
        {
            var savingsgoal = await _context.SavingsGoals.FindAsync(goalid);
            if (savingsgoal != null)
            {
                _context.SavingsGoals.Remove(savingsgoal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavingsGoalExists(int? goalid)
        {
            return _context.SavingsGoals.Any(e => e.GoalId == goalid);
        }
    }
}
