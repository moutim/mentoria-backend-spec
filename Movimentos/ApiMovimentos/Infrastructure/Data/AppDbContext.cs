using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Movimento> Movimentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Categoria table
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome)
                .HasColumnName("nome")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Configure Movimento table
        modelBuilder.Entity<Movimento>(entity =>
        {
            entity.ToTable("movimentos");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UsuarioId)
                .HasColumnName("usuario_id")
                .HasMaxLength(11)
                .IsRequired();
            entity.Property(e => e.Remetente)
                .HasColumnName("remetente")
                .HasMaxLength(11)
                .IsRequired();
            entity.Property(e => e.Destinatario)
                .HasColumnName("destinatario")
                .HasMaxLength(11)
                .IsRequired();
            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.CategoriaId)
                .HasColumnName("categoria_id");
            entity.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(255);
            entity.Property(e => e.Valor)
                .HasColumnName("valor")
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.CriadoEm)
                .HasColumnName("criado_em")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure relationship with Categoria
            entity.HasOne(m => m.Categoria)
                .WithMany(c => c.Movimentos)
                .HasForeignKey(m => m.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);
        });
            
        base.OnModelCreating(modelBuilder);
    }
}
