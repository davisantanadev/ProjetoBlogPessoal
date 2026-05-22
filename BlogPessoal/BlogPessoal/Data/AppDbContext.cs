using BlogPessoal.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Postagem> Postagens { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Tema> Temas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Postagem>()
            .HasOne(p => p.Tema)
            .WithMany(t => t.Postagens)
            .HasForeignKey("TemaID")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Postagem>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Postagens)
            .HasForeignKey("UsuarioId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
