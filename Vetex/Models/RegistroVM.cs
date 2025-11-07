namespace Vetex.Models
{
    public class OwnerPetRowVM
    {
        public int DuenoId { get; set; }
        public string Dueno { get; set; } = "";
        public string? Email { get; set; }
        public int MascotaId { get; set; }
        public string Mascota { get; set; } = "";

        public string? Telefono { get; set; }
        public string Dui { get; set; } = "";
    }

    public class RegistroListaVM
    {
        public string? OwnerName { get; set; }
        public string? PetName { get; set; }
        public List<OwnerPetRowVM> Filas { get; set; } = new();
    }

    public class HistorialItemVM
    {
        public int RegistroId { get; set; }
        public DateTime? Fecha { get; set; } // usa EF.Property si no existe en la entidad
        public string Diagnostico { get; set; } = "";
    }

    public class HistorialVM
    {
        public int DuenoId { get; set; }
        public int MascotaId { get; set; }
        public string Dueno { get; set; } = "";
        public string Mascota { get; set; } = "";
        public List<HistorialItemVM> Registros { get; set; } = new();
    }

    public class PrescItemVM
    {
        public string Medicamento { get; set; } = "";
        public int Cantidad { get; set; }
        public string Frecuencia { get; set; } = "";
    }

    public class RegistroDetalleVM
    {
        public int RegistroId { get; set; }
        public string Dueno { get; set; } = "";
        public string Mascota { get; set; } = "";

        public string Resena { get; set; } = "";
        public string Motivo { get; set; } = "";
        public decimal Temperatura { get; set; }
        public string Pulso { get; set; } = "";
        public string Respiracion { get; set; } = "";
        public string Deshidratacion { get; set; } = "";
        public string Diagnostico { get; set; } = "";

        public List<PrescItemVM> Prescripciones { get; set; } = new();

        public int DuenoId { get; set; }
        public int MascotaId { get; set; }
    }
}
