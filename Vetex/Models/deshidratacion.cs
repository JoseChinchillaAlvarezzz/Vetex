using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class deshidratacion
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string nivel { get; set; }
        public int porcentaje { get; set; }

    }
}
