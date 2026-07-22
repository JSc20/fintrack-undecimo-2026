using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack_II_Trimestre.Models
{
    public class Income
    {
        [Key]
        public int IncomeId { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int UserId { get; set; }

        // RN-02: Toda transacción debe estar vinculada obligatoriamente a una categoría
        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "El tipo de ingreso es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo de ingreso debe tener entre 2 y 50 caracteres.")]
        public string IncomeType { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto del ingreso es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal IncomeAmount { get; set; }

        [Required(ErrorMessage = "La fecha del ingreso es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime IncomeDate { get; set; }

        // Propiedad de navegación hacia Category
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
