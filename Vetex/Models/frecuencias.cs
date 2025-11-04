using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class frecuencias
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string frecuencia { get; set; }
    }
}
