using FinTrack_II_Trimestre.Data;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack_II_Trimestre.ViewComponents
{
    public class StatsStripViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public StatsStripViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return Content("");

            // Se calcula el histórico total para que nunca aparezcan en cero si existen datos.
            var incomes = _db.Incomes.Where(i => i.UserId == userId).ToList();
            var expenses = _db.Expenses.Where(e => e.UserId == userId).ToList();
            var savingsGoals = _db.SavingsGoals.Where(s => s.UserId == userId).ToList();
            
            var latestPlan = _db.BudgetPlans
                .Where(p => p.UserId == userId && p.Status)
                .OrderByDescending(p => p.CreationDate)
                .FirstOrDefault();

            decimal totalIngresos = incomes.Sum(i => i.IncomeAmount);
            decimal ingresosExtra = incomes.Where(i => !i.IsFixed).Sum(i => i.IncomeAmount);
            decimal totalEgresos = expenses.Sum(e => e.ExpenseAmount);
            decimal totalAhorro = savingsGoals.Sum(s => s.CurrentAmount);
            string planActual = latestPlan != null ? latestPlan.PlanType : "Sin plan";

            var model = new {
                TotalIngresos = totalIngresos,
                IngresosExtra = ingresosExtra,
                TotalEgresos = totalEgresos,
                TotalAhorro = totalAhorro,
                PlanActual = planActual
            };

            return View(model);
        }
    }
}
