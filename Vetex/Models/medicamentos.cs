using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class medicamentos
    {
        [Key] 
        public int id { get; set; }
        [Required]
        public string nombre { get; set; }
        public int presentacion_id { get; set; }
        public string concentracion { get; set; }
        public int via_id { get; set; }
        public int stock { get; set; }
        public decimal precio { get; set; }

    }
}
