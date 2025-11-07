using System.ComponentModel.DataAnnotations;

namespace Vetex.Models
{
    public class CrearCitaVM
    {
        // Selección
        [Required(ErrorMessage = "Seleccione una mascota.")]
        public int? MascotaId { get; set; }

        [Required(ErrorMessage = "Seleccione el tipo de cita.")]
        public int? TipoCitaId { get; set; }

        [Required(ErrorMessage = "Seleccione la fecha.")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Seleccione una hora.")]
        public int? HoraCitaId { get; set; }

        // Catálogos (siempre inicializados)
        public List<Opcion> Tipos { get; set; } = new();
        public List<Opcion> Horas { get; set; } = new();
        public List<Opcion> Mascotas { get; set; } = new();

        public int Id { get; set; }
        public string Texto { get; set; } = "";
    }

    
    
       
    


}
