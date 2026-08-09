using FinTrack_II_Trimestre.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
    public class PlanDetailController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PlanDetailController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        private IActionResult? CheckSession()
        {
            if (GetSessionUserId() == null)
                return RedirectToAction("Login", "User");
            return null;
        }

        public IActionResult Index(int? planId)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            if (planId == null) return NotFound();

            int userId = GetSessionUserId()!.Value;



            var planExists = _db.BudgetPlans.Any(p => p.PlanId == planId && p.UserId == userId);
            if (!planExists) return NotFound();

            var details = _db.PlanDetails
                .Include(pd => pd.Category)
                .Include(pd => pd.BudgetPlan)
                .Where(pd => pd.PlanId == planId)
                .ToList();

            ViewBag.PlanId = planId;
            return View(details);
        }
    }
}