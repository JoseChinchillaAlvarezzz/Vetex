using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class respiracion
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string nivel { get; set; }
        public string condicionmedica { get; set; }
    }
}
