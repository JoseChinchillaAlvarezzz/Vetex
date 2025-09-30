using Microsoft.EntityFrameworkCore;

namespace Vetex.Models
{
    public class veterinariaContext : DbContext
    {
        public veterinariaContext(DbContextOptions<veterinariaContext> options) : base(options)  
        {

        }

        public DbSet<citas> citas { get; set; }
        public DbSet<deshidratacion> deshidratacion { get; set; }
        public DbSet<detallefactura> detallefactura { get; set; }
        public DbSet<duenos> duenos { get; set; }
        public DbSet<especies> especies { get; set; }
        public DbSet<facturas> facturas { get; set; }   
        public DbSet<frecuencias> frecuencias { get; set; }
        public DbSet<horacita> horacita { get; set; }
        public DbSet<mascotas> mascotas { get; set; }
        public DbSet<medicamentos> medicamentos { get; set; }
        public DbSet<prescripcion> prescripcion { get; set; }
        public DbSet<presentacionmedicina> presentacionmedicina { get; set; }
        public DbSet<pulso> pulso { get; set; }
        public DbSet<registro> registro { get; set; }
        public DbSet<respiracion> respiracion { get; set; }
        public DbSet<sexos> sexos { get; set; }
        public DbSet<tipocita> tipocita { get; set; }
        public DbSet<usuarios> usuarios { get; set; }
        public DbSet<viaadministracion> viaadministracion { get; set; }
    }
}
