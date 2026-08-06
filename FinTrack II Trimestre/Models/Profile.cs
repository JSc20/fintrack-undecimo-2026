using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack_II_Trimestre.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }

        public int PersonId { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$", ErrorMessage = "El nombre solo debe contener letras, sin caracteres especiales ni números.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(0, 150, ErrorMessage = "La edad debe estar entre 0 y 150 años.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El número de teléfono debe contener exactamente 8 dígitos numéricos.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo debe tener un formato válido (debe incluir @)")]
        [RegularExpression(@".*\..*", ErrorMessage = "El correo debe incluir un dominio con punto (ej: .com)")]

        public string Email { get; set; } = string.Empty;
    }
}