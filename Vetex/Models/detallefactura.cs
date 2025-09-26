using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class detallefactura
    {
        [Key]
        public int id { get; set; }
        public int factura_id { get; set; }
        public int prescripcion_id { get; set; }
        public string subtotal { get; set; }
    }
}
