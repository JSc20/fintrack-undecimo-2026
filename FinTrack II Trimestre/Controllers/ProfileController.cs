using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Método auxiliar: obtener UserId de la sesión activa
        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        // Redirige a login si no hay sesión activa
        private IActionResult? CheckSession()
        {
            if (GetSessionUserId() == null)
                return RedirectToAction("Login", "User");
            return null;
        }

        // GET: Profile — muestra el perfil del usuario en sesión
        public IActionResult Index()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.Include(p => p.User).FirstOrDefault(p => p.UserId == userId);

            // Si no tiene perfil creado, redirigir a creación
            if (profile == null)
                return RedirectToAction(nameof(Create));

          
            var mesActual = DateTime.Now;
            
            // Obtener entidades con relaciones
            var incomes = _db.Incomes.Where(i => i.UserId == userId).ToList();
            var expenses = _db.Expenses.Include(e => e.Category).Where(e => e.UserId == userId).ToList();
            var savingsGoals = _db.SavingsGoals.Where(s => s.UserId == userId).ToList();
            var latestPlan = _db.BudgetPlans.Where(b => b.UserId == userId && b.Status).OrderByDescending(b => b.CreationDate).FirstOrDefault();

            var vm = new FinTrack_II_Trimestre.Models.ViewModels.ProfileIndexVM
            {
                ProfileId = profile.ProfileId,
                FullName = profile.FullName,
                Username = profile.User?.Username ?? "Desconocido",
                Email = profile.Email,
                Age = profile.Age,
                PhoneNumber = profile.PhoneNumber,
                
                TotalIncomes = incomes
                    .Where(i => i.IncomeDate.Month == mesActual.Month && i.IncomeDate.Year == mesActual.Year)
                    .Sum(i => i.IncomeAmount),
                    
                TotalExpenses = expenses
                    .Where(e => e.ExpenseDate.Month == mesActual.Month && e.ExpenseDate.Year == mesActual.Year)
                    .Sum(e => e.ExpenseAmount),
                    
                ActivePlan = latestPlan != null ? latestPlan.PlanType : "Sin plan"
            };

            // Transacciones Recientes (Combinar Incomes y Expenses)
            var recentIncomes = incomes.Select(i => new FinTrack_II_Trimestre.Models.ViewModels.RecentTransactionVM
            {
                Description = i.IncomeType,
                Date = i.IncomeDate,
                CategoryName = "Ingreso",
                Amount = i.IncomeAmount,
                IsIncome = true
            });

            var recentExpenses = expenses.Select(e => new FinTrack_II_Trimestre.Models.ViewModels.RecentTransactionVM
            {
                Description = e.ExpenseName,
                Date = e.ExpenseDate,
                CategoryName = e.Category?.CategoryName ?? "General",
                Amount = e.ExpenseAmount,
                IsIncome = false
            });

            vm.RecentTransactions = recentIncomes.Concat(recentExpenses)
                .OrderByDescending(t => t.Date)
                .Take(6)
                .ToList();

            // Meta de Ahorro Principal
            var mainGoal = savingsGoals.Where(s => s.Status).OrderBy(s => s.TargetDate).FirstOrDefault();
            if (mainGoal != null)
            {
                vm.SavingsGoalName = mainGoal.GoalName;
                vm.SavingsCurrentAmount = mainGoal.CurrentAmount;
                vm.SavingsTargetAmount = mainGoal.TargetAmount;
            }
            else
            {
                // Si no hay meta activa, mostrar total general
                vm.SavingsCurrentAmount = savingsGoals.Sum(s => s.CurrentAmount);
            }

            return View(vm);
        }

        // GET: Profile/Create
        public IActionResult Create()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Un usuario solo puede tener un perfil
            if (_db.Profiles.Any(p => p.UserId == userId))
                return RedirectToAction(nameof(Index));

            var model = new Profile();
            return View(model);
        }

        // POST: Profile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Profile profile)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Prevenir duplicado de perfil
            if (_db.Profiles.Any(p => p.UserId == userId))
            {
                TempData["ErrorMessage"] = "Ya tienes un perfil creado.";
                return RedirectToAction(nameof(Index));
            }

            // Asignar el UserId de la sesión activa
            profile.UserId = userId;

            if (ModelState.IsValid)
            {
                _db.Profiles.Add(profile);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Perfil creado exitosamente.";
                
                // Limpiar la sesión temporal
                HttpContext.Session.Remove("TempFullName");
                HttpContext.Session.Remove("TempAge");
                HttpContext.Session.Remove("TempPhone");

                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }

        // GET: Profile/Edit
        public IActionResult Edit()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.FirstOrDefault(p => p.UserId == userId);

            if (profile == null) return RedirectToAction(nameof(Create));
            return View(profile);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Profile profile)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Evitar que se edite el perfil de otro usuario
            profile.UserId = userId;

            if (ModelState.IsValid)
            {
                _db.Profiles.Update(profile);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }

        // GET: Profile/Delete
        public IActionResult Delete()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.FirstOrDefault(p => p.UserId == userId);

            if (profile == null) return NotFound();
            return View(profile);
        }

        // POST: Profile/DeleteConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var profile = _db.Profiles.FirstOrDefault(p => p.ProfileId == id && p.UserId == userId);

            if (profile == null) return NotFound();

            _db.Profiles.Remove(profile);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Perfil eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
