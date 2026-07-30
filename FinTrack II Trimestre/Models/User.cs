using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Models;

[Index(nameof(name), IsUnique = true)]
public class User
{
    [Key]
    public int id { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El nombre de usuario debe tener entre 5 y 20 caracteres.")]
    public required string name { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
    [RegularExpression(@"(?=.*[A-Z]).+", ErrorMessage = "La contraseña debe contener al menos una mayúscula.")]
    public required string password { get; set; }

    public bool status { get; set; }

    public int LoginAttempts { get; set; } = 0;
}
