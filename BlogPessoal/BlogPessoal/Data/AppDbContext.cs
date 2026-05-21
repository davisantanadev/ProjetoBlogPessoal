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

}
