using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class horacita
    {
        [Key]
        public int id { get; set; }
        [Required]
        public TimeOnly hora { get; set; }
    }
}
