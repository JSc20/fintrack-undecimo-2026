using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack_II_Trimestre.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int UserId { get; set; }

        // RN-02: Toda transacción debe estar vinculada obligatoriamente a una categoría
        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "El nombre del gasto es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre del gasto debe tener entre 2 y 50 caracteres.")]
        public string ExpenseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto del gasto es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal ExpenseAmount { get; set; }

        // Propiedad de navegación hacia Category
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
