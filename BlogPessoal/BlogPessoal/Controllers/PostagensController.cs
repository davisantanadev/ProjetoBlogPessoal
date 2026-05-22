using BlogPessoal.DTOs;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogPessoal.Controllers;

[Route("api/postagens")]
[ApiController]
[Authorize]
public class PostagensController : ControllerBase
{
    private readonly IPostagemRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITemaRepository _temaRepository;
    private readonly IIaService _iaService;

    public PostagensController(
        IPostagemRepository repository,
        IUsuarioRepository usuarioRepository,
        ITemaRepository temaRepository,
        IIaService iaService)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
        _temaRepository = temaRepository;
        _iaService = iaService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Postagem>> Get()
    {
        var postagens = _repository.GetPostagens();
        return Ok(postagens);
    }

    [HttpGet("filtro")]
    public ActionResult<IEnumerable<Postagem>> GetByFiltro([FromQuery] int? autor, [FromQuery] int? tema)
    {
        if (autor.HasValue && _usuarioRepository.GetUsuario(autor.Value) is null)
            return NotFound($"Usuário com id = {autor.Value} não encontrado");

        if (tema.HasValue && _temaRepository.GetTema(tema.Value) is null)
            return NotFound($"Tema com id = {tema.Value} não encontrado");

        var resultado = _repository.GetPostagensByFiltro(autor, tema).ToList();
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<ActionResult> Post(PostagemDTO dto, CancellationToken cancellationToken)
    {
        var tema = _temaRepository.GetTema(dto.TemaId);
        if (tema is null)
            return NotFound($"Tema com id = {dto.TemaId} não encontrado");

        var usuario = _usuarioRepository.GetUsuario(dto.UsuarioId);
        if (usuario is null)
            return NotFound($"Usuário com id = {dto.UsuarioId} não encontrado");

        var postagem = dto.ToEntity(tema, usuario);
        await EnriquecerComIaAsync(postagem, cancellationToken);

        _repository.Create(postagem);
        return Ok(postagem);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(
        int id,
        [FromBody] PostagemUpdateDTO dto,
        CancellationToken cancellationToken)
    {
        var existingPostagem = _repository.GetPostagem(id);
        if (existingPostagem is null)
            return NotFound($"Postagem com id = {id} não encontrada");

        var temaId = dto.TemaId.GetValueOrDefault() > 0
            ? dto.TemaId
            : existingPostagem.Tema?.TemaID;
        if (temaId is null)
            return BadRequest("Informe um tema valido para a postagem.");

        var usuarioId = dto.UsuarioId.GetValueOrDefault() > 0
            ? dto.UsuarioId
            : existingPostagem.Usuario?.UsuarioId;
        if (usuarioId is null)
            return BadRequest("Informe um usuario valido para a postagem.");

        var tema = _temaRepository.GetTema(temaId.Value);
        if (tema is null)
            return NotFound($"Tema com id = {temaId.Value} não encontrado");

        var usuario = _usuarioRepository.GetUsuario(usuarioId.Value);
        if (usuario is null)
            return NotFound($"Usuário com id = {usuarioId.Value} não encontrado");

        var postagem = new Postagem
        {
            PostagemId = id,
            Titulo = dto.Titulo ?? existingPostagem.Titulo,
            Texto = dto.Texto ?? existingPostagem.Texto,
            ImagemURL = dto.ImagemURL ?? existingPostagem.ImagemURL,
            Data = dto.Data ?? existingPostagem.Data,
            Tema = tema,
            Usuario = usuario
        };

        await EnriquecerComIaAsync(postagem, cancellationToken);

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

    private async Task EnriquecerComIaAsync(Postagem postagem, CancellationToken cancellationToken)
    {
        var resultadoIa = await _iaService.GerarResumoAsync(
            new IaRequestDTO
            {
                Titulo = postagem.Titulo,
                Texto = postagem.Texto
            },
            cancellationToken);

        postagem.ResumoIA = resultadoIa.Resumo;
        postagem.TagIA = resultadoIa.Tags;
        postagem.CategoriaIA = resultadoIa.Categoria;
    }
}
