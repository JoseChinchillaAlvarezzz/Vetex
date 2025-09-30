using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class mascotas
    {
        [Key]
        public int id { get; set; }
        public int dueno_id { get; set; }
        public int especie_id { get; set; }
        public string? raza { get; set; }
        public int sexo_id { get; set; }
        public int edad { get; set; }
        public int peso { get; set; }
    }
}
