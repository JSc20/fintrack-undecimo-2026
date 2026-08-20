using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using FinTrack_II_Trimestre.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FinTrack_II_Trimestre.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Método auxiliar para obtener UserId de sesión
        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        public IActionResult Index()
        {
            // Validar sesión
            int? userId = GetSessionUserId();
            if (userId == null)
                return RedirectToAction("Login", "User");

            var mesActual = DateTime.Now;

            // Obtener todas las entidades base
            var incomes = _db.Incomes.Where(i => i.UserId == userId.Value).ToList();
            var expenses = _db.Expenses.Include(e => e.Category).Where(e => e.UserId == userId.Value).ToList();
            var latestPlan = _db.BudgetPlans.Where(b => b.UserId == userId.Value).OrderByDescending(b => b.CreationDate).FirstOrDefault();
            var savingsGoals = _db.SavingsGoals.Where(s => s.UserId == userId.Value).ToList();

            // Construir el ViewModel
            var vm = new HomeDashboardVM();

            // Ingresos del Mes Actual
            vm.TotalIngresosMes = incomes
                .Where(i => i.IncomeDate.Month == mesActual.Month && i.IncomeDate.Year == mesActual.Year)
                .Sum(i => i.IncomeAmount);

            // Ingresos Extra (Ingresos no fijos)
            vm.IngresosExtra = incomes
                .Where(i => !i.IsFixed)
                .Sum(i => i.IncomeAmount);

            // Egresos del Mes Actual
            vm.TotalEgresosMes = expenses
                .Where(e => e.ExpenseDate.Month == mesActual.Month && e.ExpenseDate.Year == mesActual.Year)
                .Sum(e => e.ExpenseAmount);

            // Plan de Presupuesto
            if (latestPlan != null)
            {
                vm.PlanActual = latestPlan.PlanType;
                vm.UltimaModificacionPlan = latestPlan.CreationDate;
                // Guardar en sesión para que el Layout sidebar lo muestre en todas las vistas
                HttpContext.Session.SetString("PlanActual", latestPlan.PlanType);
            }
            else
            {
                HttpContext.Session.Remove("PlanActual");
            }

            // Metas de Ahorro
            var mainGoal = savingsGoals.Where(s => s.Status).OrderBy(s => s.TargetDate).FirstOrDefault();
            if (mainGoal != null)
            {
                vm.MontoAhorroActual = mainGoal.CurrentAmount;
                vm.MetaAhorro = mainGoal.TargetAmount;
            }
            
            // Total Ahorro global (Suma de los current de todas las metas)
            vm.TotalAhorro = savingsGoals.Sum(s => s.CurrentAmount);

            // Últimos 5 egresos (Panel de presupuestos)
            vm.UltimosEgresos = expenses
                .OrderByDescending(e => e.ExpenseDate)
                .Take(5)
                .ToList();

            // --- LOGICA DE ALERTAS: Calcular porcentaje de uso del presupuesto (RF-05) ---
            if (vm.TotalIngresosMes > 0)
            {
                vm.PorcentajePresupuestoUsado = (vm.TotalEgresosMes / vm.TotalIngresosMes) * 100;
            }
            else
            {
                vm.PorcentajePresupuestoUsado = vm.TotalEgresosMes > 0 ? 100 : 0;
            }

            // --- LOGICA DE RECORDATORIOS: Buscar egresos fijos próximos a vencer (RF-07) ---
            vm.ProximosVencimientos = expenses
                .Where(e => e.IsFixed && e.DueDate.HasValue)
                .Where(e => {
                    var daysRemaining = (e.DueDate.Value.Date - mesActual.Date).Days;
                    // Retorna egresos con 5 dias o menos de vencimiento en el mes actual
                    return daysRemaining <= 5 && e.DueDate.Value.Month == mesActual.Month && e.DueDate.Value.Year == mesActual.Year;
                })
                .OrderBy(e => e.DueDate)
                .ToList();

            // --- LOGICA DE AMORTIZACION: Calcular disponible por categoria (RN-07, RN-08) ---
            var activeCategories = _db.Categories
                .Where(c => c.UserId == userId.Value && c.CategoryStatus)
                .ToList();

            vm.CategoriasDistribucion = activeCategories.Select(cat => 
            {
                // Calcular monto asignado teóricamente a esta categoría
                decimal assignedAmount = vm.TotalIngresosMes * (cat.CategoryPercentage / 100);
                
                // Calcular cuánto se ha gastado realmente en esta categoría en el mes actual
                decimal spentAmount = expenses
                    .Where(e => e.CategoryId == cat.CategoryId && e.ExpenseDate.Month == mesActual.Month && e.ExpenseDate.Year == mesActual.Year)
                    .Sum(e => e.ExpenseAmount);

                return new CategoryDistributionVM
                {
                    CategoryName = cat.CategoryName,
                    CategoryPercentage = cat.CategoryPercentage,
                    AssignedAmount = assignedAmount,
                    SpentAmount = spentAmount,
                    AvailableAmount = assignedAmount - spentAmount
                };
            }).ToList();

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
