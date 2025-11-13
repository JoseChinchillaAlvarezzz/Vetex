using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vetex.Models
{

    public class RegistroCreateVM
    {
        // Registro
        public int? MascotaId { get; set; }
        public string? Resena { get; set; }
        public string? Motivo { get; set; }
        public string? Diagnostico { get; set; }

        // Prescripciones seleccionadas en el modal (se envían desde JS)
        public List<PrescripcionItemVM> Prescripciones { get; set; } = new();
    }

    public class PrescripcionItemVM
    {
        public int MedicamentoId { get; set; }
        public string? Dosis { get; set; }        // ej. "1 tableta"
        public int? FrecuenciaId { get; set; }    // relaciona con tabla frecuencias
        public string? Duracion { get; set; }     // ej. "5 días"
        public string? Indicacion { get; set; }   // texto libre
        public int? Cantidad { get; set; }        // opcional
        public string? Display { get; set; }      // para mostrar en chips (no se guarda)
    }

}
