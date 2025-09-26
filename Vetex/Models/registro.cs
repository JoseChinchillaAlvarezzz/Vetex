using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class registro
    {
        [Key]
        public int id { get; set; }
        public int mascota_id { get; set; }
        public string resena { get; set; }
        public string motivo { get; set; }
        public decimal temperatura { get; set; }
        public int pulso_id { get; set; }
        public int respiracion_id { get; set; }
        public int deshidratacion_id { get; set; }
        public string diagnostico { get; set; }
        public bool pagadoaqui { get; set; }
    }
}
