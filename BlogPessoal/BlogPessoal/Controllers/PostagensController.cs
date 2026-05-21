using BlogPessoal.Data;
using BlogPessoal.DTOs;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Controllers
{
    [Route("api/postagens")]
    [ApiController]
    public class PostagensController : ControllerBase
    {
        private readonly IPostagemRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITemaRepository _temaRepository;

        public PostagensController(IPostagemRepository repository,
            IUsuarioRepository usuarioRepository,
            ITemaRepository temaRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _temaRepository = temaRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Postagem>> Get()
        {
            var postagens = _repository.GetPostagens();
            if (!postagens.Any())
                return NotFound("Nenhuma postagem encontrada.");
            return Ok(postagens);
        }

        [HttpGet("filtro")]
        public ActionResult<IEnumerable<Postagem>> GetByFiltro([FromQuery] int? autor, [FromQuery] int? tema)
        {
            var resultado = _repository.GetPostagensByFiltro(autor, tema).ToList();
            if (!resultado.Any())
                return NotFound("Nenhuma postagem encontrada para os filtros informados.");
            return Ok(resultado);
        }

        [HttpPost]
        public ActionResult Post(PostagemDTO dto)
        {
            var tema = _temaRepository.GetTema(dto.TemaId);
            if (tema is null)
                return NotFound($"Tema com id = {dto.TemaId} não encontrado");

            var usuario = _usuarioRepository.GetUsuario(dto.UsuarioId);
            if (usuario is null)
                return NotFound($"Usuário com id = {dto.UsuarioId} não encontrado");

            var postagem = dto.ToEntity(tema, usuario);
            _repository.Create(postagem);
            return Ok(postagem);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, PostagemDTO dto)
        {
            var existingPostagem = _repository.GetPostagem(id);
            if (existingPostagem is null)
                return NotFound($"Postagem com id = {id} não encontrada");

            var tema = _temaRepository.GetTema(dto.TemaId);
            if (tema is null)
                return NotFound($"Tema com id = {dto.TemaId} não encontrado");

            var usuario = _usuarioRepository.GetUsuario(dto.UsuarioId);
            if (usuario is null)
                return NotFound($"Usuário com id = {dto.UsuarioId} não encontrado");

            var postagem = dto.ToEntity(tema, usuario);
            postagem.PostagemId = id;
            _repository.Update(id, postagem);
            return Ok(postagem);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var postagem = _repository.Delete(id);
            if (postagem is null)
                return NotFound($"Postagem com id = {id} não encontrada");
            return Ok(postagem);
        }
    }
}
