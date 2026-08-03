using System.ComponentModel.DataAnnotations;

namespace FinTrack_II_Trimestre.Models
{
    public class Users
    {
        public int userId { get; set; }

        [Required(ErrorMessage = "Debe ingresar su nombre de usuario.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 20 caracteres.")]
        public string username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Debe ingresar su constraseña.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$", ErrorMessage =
            "La contraseña debe tener al menos 8 caracteres, incluyendo una letra mayúscula y una letra minúscula como mínimo.")]
        public string password { get; set; } = string.Empty;
        public bool status { get; set; }
        public int failedAttempts { get; set; } = 0;
        public DateTime? lockoutEndDate { get; set; }


    }
}
