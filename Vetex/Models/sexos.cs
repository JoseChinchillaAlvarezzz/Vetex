using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class sexos
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string sexo { get; set; }
    }
}
