using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class presentacionmedicina
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string presentacion { get; set; }
    }
}
