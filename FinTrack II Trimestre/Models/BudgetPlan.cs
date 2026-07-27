using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack_II_Trimestre.Models
{
    public class BudgetPlan
    {
        [Key]
        public int PlanId { get; set; }

        //Poner en función la relación con la tabla User, pero por ahora se deja comentada.
        // [ForeignKey(nameof(UserId))]
        // public virtual User? User { get; set; }

        [Required(ErrorMessage = "Seleccionar un tipo de plan es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo de plan debe tener entre 2 y 50 caracteres.")]
        public string PlanType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccionar una fecha de creación es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;
    }
}