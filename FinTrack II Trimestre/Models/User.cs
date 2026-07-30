using System.ComponentModel.DataAnnotations;

namespace FinTrack_II_Trimestre.Models;
public class Use
{
    [Key]
    public int id { get; set; }
    public required string name { get; set; }
    public required string password { get; set; }
    public bool status { get; set; }
}
