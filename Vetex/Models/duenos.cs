using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class duenos
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string nombre { get; set; }
        public string telefono { get; set; }
        [Required]
        public string dui { get; set; }
        public string direccion { get; set; }
    }
}
