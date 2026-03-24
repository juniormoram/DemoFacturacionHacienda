using DemoFacturacionHacienda.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoFacturacionHacienda.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Factura> Facturas { get; set; }
        public DbSet<LineaDetalle> LineasDetalle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Factura>()
                .HasMany(f => f.Lineas)
                .WithOne(l => l.Factura)
                .HasForeignKey(l => l.FacturaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimales
            modelBuilder.Entity<Factura>()
                .Property(f => f.TotalVenta).HasPrecision(18, 5);
            modelBuilder.Entity<Factura>()
                .Property(f => f.TotalImpuesto).HasPrecision(18, 5);
            modelBuilder.Entity<Factura>()
                .Property(f => f.TotalComprobante).HasPrecision(18, 5);

            modelBuilder.Entity<LineaDetalle>()
                .Property(l => l.PrecioUnitario).HasPrecision(18, 5);
            modelBuilder.Entity<LineaDetalle>()
                .Property(l => l.Cantidad).HasPrecision(18, 5);
        }
    }
}
