using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class prescripcion
    {
        [Key]
        public int id { get; set; }
        public int registro_id { get; set; }
        public int medicamento_id { get; set; }
        public string dosis { get; set; }
        public int frecuencia_id { get; set; }
        public string duracion { get; set; }
        public string indicacion { get; set; }
    }
}
