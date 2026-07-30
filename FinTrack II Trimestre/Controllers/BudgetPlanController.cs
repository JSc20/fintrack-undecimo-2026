using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinTrack_II_Trimestre.Controllers;

[Authorize]
public class BudgetPlanController : Controller
{
    private readonly ApplicationDbContext _db;

    public BudgetPlanController(ApplicationDbContext db)
    {
        _db = db;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    public async Task<IActionResult> Index()
    {
        var plans = await _db.BudgetPlans
            .Where(p => p.UserId == GetUserId())
            .Include(p => p.PlanDetails)
            .ThenInclude(pd => pd.Category)
            .ToListAsync();
        return View(plans);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var plan = await _db.BudgetPlans
            .Include(p => p.PlanDetails)
            .ThenInclude(pd => pd.Category)
            .FirstOrDefaultAsync(p => p.PlanId == id && p.UserId == GetUserId());

        if (plan == null) return NotFound();

        return View(plan);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BudgetPlan plan)
    {
        if (ModelState.IsValid)
        {
            plan.UserId = GetUserId();
            plan.CreationDate = DateTime.Now;

            var activePlan = await _db.BudgetPlans
                .FirstOrDefaultAsync(p => p.UserId == plan.UserId && p.Status);

            if (activePlan != null)
            {
                activePlan.Status = false;
            }

            _db.BudgetPlans.Add(plan);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id)
    {
        var plan = await _db.BudgetPlans
            .FirstOrDefaultAsync(p => p.PlanId == id && p.UserId == GetUserId());

        if (plan == null) return NotFound();

        var activePlan = await _db.BudgetPlans
            .FirstOrDefaultAsync(p => p.UserId == GetUserId() && p.Status && p.PlanId != id);

        if (activePlan != null)
        {
            activePlan.Status = false;
        }

        plan.Status = true;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        var plan = await _db.BudgetPlans
            .FirstOrDefaultAsync(p => p.PlanId == id && p.UserId == GetUserId());

        if (plan == null) return NotFound();

        plan.Status = false;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var plan = await _db.BudgetPlans
            .FirstOrDefaultAsync(p => p.PlanId == id && p.UserId == GetUserId());

        if (plan != null)
        {
            _db.BudgetPlans.Remove(plan);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
