using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class especies
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string especie { get; set; }
    }
}
