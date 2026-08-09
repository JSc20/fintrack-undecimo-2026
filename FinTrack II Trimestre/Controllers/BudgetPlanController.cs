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
        public IActionResult Create()
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            // Tipos de planes permitidos en el sistema
            ViewBag.PlanTypes = new SelectList(new[] { "50-30-20", "6x6", "Personalizado" });
            ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus), "CategoryId", "CategoryName");

            return View();
        }

        // POST: BudgetPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BudgetPlan budgetPlan, List<int> categoryIds)
        {
            var redirect = CheckSession();
            if (redirect != null) return redirect;

            int userId = GetSessionUserId()!.Value;

            if (string.IsNullOrEmpty(budgetPlan.PlanType))
                ModelState.AddModelError("PlanType", "El tipo de plan es obligatorio.");

            // Validaciones específicas para el plan personalizado
            if (budgetPlan.PlanType == "Personalizado")
            {
                if (categoryIds == null || !categoryIds.Any())
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos una categoría para el plan personalizado.");
                }
                else
                {
                    // RN-01: El plan personalizado debe incluir obligatoriamente una categoría de Ahorro
                    bool hasSavingsCategory = categoryIds.Any(catId =>
                        _db.Categories.Any(c => c.CategoryId == catId && c.CategoryName.ToLower().Contains("ahorro")));

                    if (!hasSavingsCategory)
                    {
                        ModelState.AddModelError("", "RN-01: El plan personalizado debe incluir obligatoriamente una categoría de Ahorro.");
                    }

                    // RN-04: La sumatoria de los porcentajes de las categorías seleccionadas debe ser exactamente 100%
                    decimal totalPercentage = 0;
                    foreach (var catId in categoryIds)
                    {
                        var cat = _db.Categories.Find(catId);
                        if (cat != null)
                        {
                            totalPercentage += cat.CategoryPercentage;
                        }
                    }

                    if (totalPercentage != 100.00m)
                    {
                        ModelState.AddModelError("", $"RN-04: La sumatoria de los porcentajes de las categorías debe ser exactamente 100%. Actualmente suma {totalPercentage}%.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PlanTypes = new SelectList(new[] { "50-30-20", "6x6", "Personalizado" }, budgetPlan.PlanType);
                ViewBag.Categories = new SelectList(_db.Categories.Where(c => c.CategoryStatus), "CategoryId", "CategoryName");
                return View(budgetPlan);
            }

            // RN-06: Un plan activo por usuario. Desactivar planes anteriores antes de registrar el nuevo.
            var activePlans = _db.BudgetPlans.Where(p => p.UserId == userId && p.Status).ToList();
            foreach (var plan in activePlans)
            {
                plan.Status = false;
                _db.BudgetPlans.Update(plan);
            }

            // Configurar propiedades del nuevo plan
            budgetPlan.UserId = userId;
            budgetPlan.CreationDate = DateTime.Now;
            budgetPlan.Status = true; // Activo por defecto

            _db.BudgetPlans.Add(budgetPlan);
            _db.SaveChanges(); // Guarda para generar el PlanId

            // Registrar los detalles en la tabla puente PlanDetail
            if (budgetPlan.PlanType == "Personalizado" && categoryIds != null)
            {
                foreach (var catId in categoryIds)
                {
                    var detail = new PlanDetail
                    {
                        PlanId = budgetPlan.PlanId,
                        CategoryId = catId
                    };
                    _db.PlanDetails.Add(detail);
                }
                _db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Plan financiero creado y activado exitosamente.";
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
    }
}