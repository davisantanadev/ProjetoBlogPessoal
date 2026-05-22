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
    public Usuario? GetUsuario(int id)
    {
        return _context.Usuarios.FirstOrDefault(u => u.UsuarioId == id);
    }

    public Usuario? GetUsuarioByEmail(string email)
    {
        return _context.Usuarios
            .FirstOrDefault(u => u.Email != null && u.Email.ToLower() == email.ToLower());
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
        var existing = _context.Usuarios.Find(id);
        if (existing is null)
            throw new ArgumentNullException(nameof(existing));

        existing.Nome = user.Nome;
        existing.Email = user.Email;
        existing.Senha = user.Senha;
        existing.FotoURL = user.FotoURL;

        _context.SaveChanges();
        return existing;
    }
    public Usuario? Delete(int id)
    {
        var user = _context.Usuarios.Find(id);
        if (user is null)
        {
            return null;
        }

        var userPosts = _context.Postagens
            .Where(p => EF.Property<int>(p, "UsuarioId") == id)
            .ToList();

        if (userPosts.Any())
        {
            _context.Postagens.RemoveRange(userPosts);
        }

        _context.Usuarios.Remove(user);
        _context.SaveChanges();
        return user;
    }
}
