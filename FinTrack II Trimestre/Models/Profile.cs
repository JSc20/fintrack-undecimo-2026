using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FinTrack_II_Trimestre.Models
{
    // Modelo base de Profile — El compañero (Anpher) completará este modelo
    // con las validaciones y el ProfileController correspondiente.
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }
        public int PersonId { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string FullName { get; set; } = string.Empty;
        [Range(0, 150, ErrorMessage = "La edad debe estar entre 0 y 150.")]
        public int Age { get; set; }
        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        public int PhoneNumber { get; set; }
    }
}