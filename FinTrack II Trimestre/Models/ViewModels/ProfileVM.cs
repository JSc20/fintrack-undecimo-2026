namespace FinTrack_II_Trimestre.Models.ViewModels
{
    public class ProfileViewModel
    {
        public Profile Perfil { get; set; } = new Profile();
        public string PlanType { get; set; } = "Sin plan";
        public DateTime? FechaCreacion { get; set; }
    }
}
