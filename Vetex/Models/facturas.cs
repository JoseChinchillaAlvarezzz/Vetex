using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class facturas
    {
        [Key]
        public int id { get; set; }
        public int dueno_id { get; set; }
        public DateTime fecha { get; set; }
        public decimal total { get; set; }
    }
}
