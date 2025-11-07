namespace Vetex.Models
{
    public class CitaDelDiaVM
    {
        public int Codigo { get; set; }          // ID de la cita
        public string TipoCita { get; set; } = ""; // Tipo de cita (Cirugía, Aseo, etc.)
        public string Dueno { get; set; } = "";    // Nombre del dueño
        public string Mascota { get; set; } = "";  // Nombre de la mascota
        public string Hora { get; set; } = "";     // Hora formateada (ej. 8:00 AM)

        public DateTime Fecha { get; set; }
    }
}
