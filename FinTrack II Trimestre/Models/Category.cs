using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack_II_Trimestre.Models
{
    // Modelo base de Category — El compañero (Anpher) completará este modelo
    // con las validaciones y el CategoryController correspondiente.
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        public string CategoryName { get; set; } = string.Empty;

        public bool CategoryStatus { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CategoryPercentage { get; set; }
    }
}
