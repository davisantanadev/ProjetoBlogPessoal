using BlogPessoal.Data;
using BlogPessoal.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }
    public Usuario GetUsuario(int id)
    {
        return _context.Usuarios.FirstOrDefault(u => u.UsuarioId == id);
    }
    public Usuario Create(Usuario user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        _context.Usuarios.Add(user);
        _context.SaveChanges();
        return user;
    }
    public Usuario Update(int id, Usuario user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        _context.Entry(user).State = EntityState.Modified;
        _context.SaveChanges();
        return user;
    }
    public Usuario Delete(int id)
    {
        var user = _context.Usuarios.Find(id);
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        _context.Remove(user);
        _context.SaveChanges();
        return user;
    }
}
