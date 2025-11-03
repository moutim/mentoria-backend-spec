using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Conta> Contas { get; set; }
    public DbSet<Movimento> Movimentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<Movimento>()
            .HasOne(m => m.Conta)
            .WithMany(c => c.Movimentos)
            .HasForeignKey(m => m.ContaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure decimal precision
        modelBuilder.Entity<Conta>()
            .Property(c => c.Saldo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Movimento>()
            .Property(m => m.Valor)
            .HasPrecision(18, 2);
            
        base.OnModelCreating(modelBuilder);
    }
}
