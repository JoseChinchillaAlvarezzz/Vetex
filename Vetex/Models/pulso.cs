using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class pulso
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string nivel { get; set; }

    }
}
