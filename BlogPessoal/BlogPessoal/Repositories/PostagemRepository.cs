using BlogPessoal.Data;
using BlogPessoal.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Repositories;

public class PostagemRepository : IPostagemRepository
{
    private readonly AppDbContext _context;

    public PostagemRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Postagem> GetPostagens()
    {
        return _context.Postagens.AsNoTracking().ToList();
    }

    public Postagem GetPostagem(int id)
    {
        return _context.Postagens.FirstOrDefault(p => p.PostagemId == id);
    }

    public Postagem Delete(int id)
    {
        var postagem = _context.Postagens.Find(id);
        if (postagem is null)
        {
            return null;
        }
        _context.Remove(postagem);
        _context.SaveChanges();
        return postagem;
    }

    public IEnumerable<Postagem> GetPostagensByFiltro(int? autor, int? tema)
    {
        IQueryable<Postagem> query = _context.Postagens
            .Include(p => p.Usuario)
            .Include(p => p.Tema);

        if (autor.HasValue)
            query = query.Where(p => p.Usuario != null && p.Usuario.UsuarioId == autor.Value);

        if (tema.HasValue)
            query = query.Where(p => p.Tema != null && p.Tema.TemaID == tema.Value);

        return query.ToList();
    }

    public Postagem Create(Postagem postagem)
    {
        _context.Postagens.Add(postagem);
        _context.SaveChanges();
        return postagem;
    }

    public Postagem Update(int id, Postagem postagem)
    {
        _context.Entry(postagem).State = EntityState.Modified;
        _context.SaveChanges();
        return postagem;
    }
}
