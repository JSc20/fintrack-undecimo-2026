using System.ComponentModel.DataAnnotations;

namespace FinTrack_II_Trimestre.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Debe ingresar su nombre de usuario.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar su contraseña.")]
        public string Password { get; set; } = string.Empty;
    }
}
