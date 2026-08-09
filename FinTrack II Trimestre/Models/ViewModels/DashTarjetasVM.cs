namespace FinTrack_II_Trimestre.Models.ViewModels
{
    // Resumen financiero (Son las tarjetas superiores en la mayoria de las vistas) 
    public class DashboardStatsViewModel
    {
        public decimal Ingresos { get; set; }
        public decimal Egresos { get; set; }
        public string PlanType { get; set; } = "Sin plan";
        public int CantidadEgresos { get; set; }
    }
}
