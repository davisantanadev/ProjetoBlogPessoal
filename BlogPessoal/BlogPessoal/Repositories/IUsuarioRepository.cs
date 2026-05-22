using BlogPessoal.Models;

namespace BlogPessoal.Repositories;

public interface IUsuarioRepository
{
    Usuario? GetUsuario(int id);
    Usuario? GetUsuarioByEmail(string email);
    Usuario Create(Usuario user);
    Usuario Update(int id, Usuario user);
    Usuario? Delete(int id);
}
