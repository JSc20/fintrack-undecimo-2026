namespace FinTrack_II_Trimestre.Models.ViewModels
{
        // Para la vista Usurio
        public class UsuarioViewModel
        {
            // Datos del form de perfil
            public string Nombre { get; set; } = "";
            public int Edad { get; set; }
            public string Telefono { get; set; } = "";

            // Tarjetas de resumen de los datos
            public decimal Ingresos { get; set; }
            public decimal Egresos { get; set; }
            public string PlanActual { get; set; } = "";
            public decimal Ahorro { get; set; }
        }
    }
