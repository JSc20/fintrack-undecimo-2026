using System.ComponentModel.DataAnnotations;

namespace FinTrack_II_Trimestre.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [Display(Name = "Nombre de usuario")]
    public required string name { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public required string password { get; set; }
}
