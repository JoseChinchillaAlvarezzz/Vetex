using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class viaadministracion
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string via { get; set; }
    }
}
