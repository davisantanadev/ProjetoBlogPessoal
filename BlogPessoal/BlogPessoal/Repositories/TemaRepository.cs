using BlogPessoal.Data;
using BlogPessoal.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Repositories;

public class TemaRepository : ITemaRepository
{
    private readonly AppDbContext _context;

    public TemaRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Tema> GetTemas()
    {
        return _context.Temas.ToList();
    }
    public Tema? GetTema(int id)
    {
        return _context.Temas.FirstOrDefault(t => t.TemaID == id);
    }
    public Tema Create(Tema tema)
    {
        if(tema is null)
        {
            throw new ArgumentNullException(nameof(tema));
        }
        _context.Temas.Add(tema);
        _context.SaveChanges();
        return tema;
    }
    public Tema Update(int id, Tema tema)
    {
        if (tema is null)
        {
            throw new ArgumentNullException(nameof(tema));
        }
        _context.Entry(tema).State = EntityState.Modified;
        _context.SaveChanges();
        return tema;
    }
    public Tema? Delete(int id)
    {
        var tema = _context.Temas.Find(id);
        if (tema is null)
        {
            return null;
        }
        _context.Remove(tema);
        _context.SaveChanges();
        return tema;
    }
}
