using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Controllers
{
    public class BudgetPlanController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BudgetPlanController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Método auxiliar: Obtener el ID del usuario en sesión
        private int? GetSessionUserId() => HttpContext.Session.GetInt32("UserId");

        // Método auxiliar: Validar si hay sesión activa, de lo contrario redirige al Login
        private IActionResult? CheckSession()
        {
            if (GetSessionUserId() == null)
                return RedirectToAction("Login", "User");
            return null;
        }

        // GET: BudgetPlan — Muestra el historial y plan activo del usuario
        public IActionResult Index()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var plans = _db.BudgetPlans
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreationDate)
                .ToList();

            return View(plans);
        }

        // GET: BudgetPlan/Create
        public IActionResult Create(string? planType = null)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            // Tipos de planes permitidos en el sistema
            ViewBag.PlanTypes = new SelectList(new[] { "50-30-20", "6x6", "Personalizado" }, planType);
            ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.UserId == userId), "CategoryId", "CategoryName");
            ViewBag.PreselectedPlanType = planType;

            return View();
        }

        // POST: BudgetPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BudgetPlan budgetPlan)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            if (string.IsNullOrEmpty(budgetPlan.PlanType))
                ModelState.AddModelError("PlanType", "El tipo de plan es obligatorio.");

            if (!ModelState.IsValid)
            {
                ViewBag.PlanTypes = new SelectList(new[] { "50-30-20", "6x6", "Personalizado" }, budgetPlan.PlanType);
                ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.UserId == userId), "CategoryId", "CategoryName");
                return View(budgetPlan);
            }

            // RN-06: Un plan activo por usuario. Desactivar planes anteriores antes de registrar el nuevo.
            var activePlans = _db.BudgetPlans.Where(p => p.UserId == userId && p.Status).ToList();
            foreach (var plan in activePlans)
            {
                plan.Status = false;
                _db.BudgetPlans.Update(plan);
            }

            // Desactivar las categorías activas anteriores. Sólo las correspondientes a este plan quedarán activas.
            var activeCategories = _db.Categories.Where(c => c.UserId == userId && c.CategoryStatus).ToList();
            foreach (var cat in activeCategories)
            {
                cat.CategoryStatus = false;
                _db.Categories.Update(cat);
            }

            // Configurar propiedades del nuevo plan
            budgetPlan.UserId = userId;
            budgetPlan.CreationDate = DateTime.Now;
            budgetPlan.Status = true; // Activo por defecto

            _db.BudgetPlans.Add(budgetPlan);
            _db.SaveChanges(); // Guarda para generar el PlanId

            // Generar categorías y detalles de forma automática SOLAMENTE para planes fijos
            if (budgetPlan.PlanType == "50-30-20" || budgetPlan.PlanType == "6x6")
            {
                var categoriesToCreate = new List<(string Name, decimal Percentage)>();

                if (budgetPlan.PlanType == "50-30-20")
                {
                    categoriesToCreate.Add(("Necesidades", 50m));
                    categoriesToCreate.Add(("Gastos personales", 30m));
                    categoriesToCreate.Add(("Ahorro", 20m));
                }
                else if (budgetPlan.PlanType == "6x6")
                {
                    categoriesToCreate.Add(("Necesidades Básicas", 55m));
                    categoriesToCreate.Add(("Ahorro a Largo Plazo", 10m));
                    categoriesToCreate.Add(("Educación", 10m));
                    categoriesToCreate.Add(("Ocio y Diversión", 10m));
                    categoriesToCreate.Add(("Libertad Financiera", 10m));
                    categoriesToCreate.Add(("Donativos", 5m));
                }

                foreach (var cat in categoriesToCreate)
                {
                    var existingCategory = _db.Categories.FirstOrDefault(c => c.UserId == userId && c.CategoryName.ToLower() == cat.Name.ToLower());
                    int catId;

                    if (existingCategory == null)
                    {
                        var newCategory = new Category
                        {
                            UserId = userId,
                            CategoryName = cat.Name,
                            CategoryPercentage = cat.Percentage,
                            CategoryStatus = true
                        };
                        _db.Categories.Add(newCategory);
                        _db.SaveChanges(); // Para obtener el CategoryId
                        catId = newCategory.CategoryId;
                    }
                    else
                    {
                        // Actualizar estado y porcentaje si ya existía pero con otros valores
                        existingCategory.CategoryStatus = true;
                        existingCategory.CategoryPercentage = cat.Percentage;
                        _db.Categories.Update(existingCategory);
                        _db.SaveChanges();
                        catId = existingCategory.CategoryId;
                    }

                    var detail = new PlanDetail
                    {
                        PlanId = budgetPlan.PlanId,
                        CategoryId = catId
                    };
                    _db.PlanDetails.Add(detail);
                }
                _db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Plan financiero creado exitosamente.";
            // Actualizar sesión para que el Layout sidebar refleje el nuevo plan
            HttpContext.Session.SetString("PlanActual", budgetPlan.PlanType);

            // Redirección según el tipo de plan
            if (budgetPlan.PlanType == "Personalizado")
            { 
                return RedirectToAction("Index", "Category");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: BudgetPlan/Details/5
        public IActionResult Details(int? id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            if (id == null || id == 0) return NotFound();

            int userId = GetSessionUserId()!.Value;
            var plan = _db.BudgetPlans
                .FirstOrDefault(p => p.PlanId == id && p.UserId == userId);

            if (plan == null) return NotFound();

            // Cargar los detalles asociados al plan con sus respectivas categorías
            var details = _db.PlanDetails
                .Include(pd => pd.Category)
                .Where(pd => pd.PlanId == id)
                .ToList();

            ViewBag.PlanDetails = details;
            return View(plan);
        }

        // GET: BudgetPlan/Delete/5
        public IActionResult Delete(int? id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            if (id == null || id == 0) return NotFound();

            int userId = GetSessionUserId()!.Value;
            var plan = _db.BudgetPlans.FirstOrDefault(p => p.PlanId == id && p.UserId == userId);

            if (plan == null) return NotFound();
            return View(plan);
        }

        // POST: BudgetPlan/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;
            var plan = _db.BudgetPlans.FirstOrDefault(p => p.PlanId == id && p.UserId == userId);

            if (plan == null) return NotFound();

            // Desactivar o eliminar dependencias si es necesario, 
            // EF Core borrará en cascada los detalles si está configurado así.
            _db.BudgetPlans.Remove(plan);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Plan eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}