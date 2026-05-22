using BlogPessoal.Models;

namespace BlogPessoal.Repositories;

public interface IPostagemRepository
{
    IEnumerable<Postagem> GetPostagens();
    Postagem? GetPostagem(int id);
    IEnumerable<Postagem> GetPostagensByFiltro(int? autor, int? tema);
    Postagem Create(Postagem postagem);
    Postagem Update(int id, Postagem postagem);
    Postagem? Delete(int id);
}
