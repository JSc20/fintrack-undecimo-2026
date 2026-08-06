using System.ComponentModel.DataAnnotations;

namespace FinTrack_II_Trimestre.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Debe ingresar su nombre de usuario.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 20 caracteres.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar su contraseña.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$", ErrorMessage =
            "La contraseña debe tener al menos 8 caracteres, incluyendo una letra mayúscula y una letra minúscula como mínimo.")]
        public string Password { get; set; } = string.Empty;

        public bool Status { get; set; }

        public int LoginAttempts { get; set; } = 0;

        public DateTime? LockoutEndDate { get; set; }
    }
}
