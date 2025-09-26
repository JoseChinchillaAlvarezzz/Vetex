using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class tipocita
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string tipo { get; set; }
    }
}
