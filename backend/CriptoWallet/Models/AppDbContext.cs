using Microsoft.EntityFrameworkCore;
using CriptoWallet.Api.Models;

namespace CriptoWallet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Criptomoneda> Criptomonedas { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Cliente)
                .WithMany(c => c.Transacciones)
                .HasForeignKey(t => t.ClienteID);

            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Criptomoneda)
                .WithMany(c => c.Transacciones)
                .HasForeignKey(t => t.CryptoCode);

            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Exchange);
        }
    }
}
