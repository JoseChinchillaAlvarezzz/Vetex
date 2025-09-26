using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class citas
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int mascota_id { get; set; }
        [Required]
        public DateTime fecha { get; set; }
        public int horacita_id { get; set; }
        public int tipocita_id { get; set; }
    }
}
