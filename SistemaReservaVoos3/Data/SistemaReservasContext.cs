using Microsoft.EntityFrameworkCore;
using SistemaReservasVoos1.Modelos;
using SistemaReservasVoos1.;
namespace SistemaReservasVoos1.Data
{
    public class SistemaReservasContext : DbContext
    {
        public SistemaReservasContext(DbContextOptions<SistemaReservasContext> options) : base(options) { }

        public DbSet<Passageiro> Passageiros { get; set; }
        public DbSet<Voo> Voos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Passageiro>().HasKey(p => p.CPF);
            modelBuilder.Entity<Voo>().HasKey(v => v.Id);
            modelBuilder.Entity<Reserva>().HasKey(r => r.Id);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Passageiro)
                .WithMany()
                .HasForeignKey(r => r.CPFPassageiro);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Voo)
                .WithMany()
                .HasForeignKey(r => r.VooId);
            modelBuilder.Entity<Voo>()
            .Property(v => v.CapacidadeTotal)
            .IsRequired()
            .HasDefaultValue(100);

        }

    }

}
    
    

