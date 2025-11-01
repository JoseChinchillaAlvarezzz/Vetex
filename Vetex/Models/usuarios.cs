using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class usuarios
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string usuario { get; set; }
        [Required]
        public string contrasenia { get; set; }
    }
}
