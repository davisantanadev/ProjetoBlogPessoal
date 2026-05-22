using BlogPessoal.Models;

namespace BlogPessoal.Services;

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}
