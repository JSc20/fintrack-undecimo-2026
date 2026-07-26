using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack_II_Trimestre.Models
{
	
	public class SavingsGoal
	{
		[Key]
		public int GoalId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required(ErrorMessage = "El nombre de la meta es obligatorio.")]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
		public string GoalName { get; set; } = string.Empty;

		[Required(ErrorMessage = "El monto objetivo es obligatorio.")]
		[Column(TypeName = "decimal(18,2)")]
		[Range(0.01, double.MaxValue, ErrorMessage = "El monto objetivo debe ser mayor a 0.")]
		public decimal TargetAmount { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		[Range(0, double.MaxValue, ErrorMessage = "El monto actual debe ser mayor o igual a 0.")]
		public decimal CurrentAmount { get; set; }

		[Required(ErrorMessage = "La fecha meta es obligatoria.")]
		[DataType(DataType.Date)]
		public DateTime TargetDate { get; set; }

		public bool Status { get; set; }
	}
}