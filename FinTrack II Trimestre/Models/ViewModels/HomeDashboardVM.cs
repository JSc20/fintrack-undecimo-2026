using FinTrack_II_Trimestre.Models;

namespace FinTrack_II_Trimestre.Models.ViewModels
{
    public class HomeDashboardVM
    {
        public decimal TotalIngresosMes { get; set; }
        public decimal IngresosExtra { get; set; }
        public decimal TotalEgresosMes { get; set; }
        public string PlanActual { get; set; } = "Sin plan";
        public decimal TotalAhorro { get; set; }
        
        // Alerta de Presupuesto (RF-05, HU-19/20)
        public decimal PorcentajePresupuestoUsado { get; set; }
        
        public List<Expense> UltimosEgresos { get; set; } = new List<Expense>();
        
        // Recordatorios de Vencimientos (RF-07, HU-21/22)
        public List<Expense> ProximosVencimientos { get; set; } = new List<Expense>();
        
        // Amortización Automática (RN-07, RN-08)
        public List<CategoryDistributionVM> CategoriasDistribucion { get; set; } = new List<CategoryDistributionVM>();
        
        public DateTime? UltimaModificacionPlan { get; set; }
        
        // Ahorro Principal
        public decimal MontoAhorroActual { get; set; }
        public decimal MetaAhorro { get; set; }
    }

    // ViewModel auxiliar para la distribución por categoría
    public class CategoryDistributionVM
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal CategoryPercentage { get; set; }
        public decimal AssignedAmount { get; set; } // Monto asignado según el ingreso
        public decimal SpentAmount { get; set; }    // Monto gastado en el mes
        public decimal AvailableAmount { get; set; } // Disponible neto (Asignado - Gastado)
    }
}
