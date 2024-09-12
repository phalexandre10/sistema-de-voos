using Microsoft.EntityFrameworkCore;
using ReservaPassagens.Modelos;

namespace ReservaPassagens.Data
{

    // DbContext
    public class ContextoAereo : DbContext
    {
        public ContextoAereo(DbContextOptions<ContextoAereo> options) : base(options) { }
        public DbSet<Passageiro> Passageiros { get; set; }
        public DbSet<Voo> Voos { get; set; }
        public DbSet<Assento> Assentos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Bilhete> Bilhetes { get; set; }
    }

}
