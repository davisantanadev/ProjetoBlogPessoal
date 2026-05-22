using BlogPessoal.Models;

namespace BlogPessoal.Repositories;

public interface ITemaRepository
{
    IEnumerable<Tema> GetTemas();
    Tema? GetTema(int id);
    Tema Create(Tema tema);
    Tema Update(int id, Tema tema);
    Tema? Delete(int id);
}
