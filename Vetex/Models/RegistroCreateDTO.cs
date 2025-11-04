namespace Vetex.Models
{
    public class RegistroMedDTO
    {
        public int MedicamentoId { get; set; }
        public int Cantidad { get; set; }
        public int DosisHoras { get; set; }
    }

    public class RegistroCreateDTO
    {
        // Relación principal
        public int MascotaId { get; set; }

        // Ficha clínica (según tu entidad)
        public string? Resena { get; set; }
        public string? Motivo { get; set; }
        public decimal Temperatura { get; set; }
        public int PulsoId { get; set; }
        public int RespiracionId { get; set; }
        public int DeshidratacionId { get; set; }
        public string? Diagnostico { get; set; }

        // Prescripciones
        public List<RegistroMedDTO> Medicamentos { get; set; } = new();
    }
}
