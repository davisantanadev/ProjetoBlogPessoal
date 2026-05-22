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
    public async Task<ActionResult> Put(int id, CancellationToken cancellationToken)
    {
        string rawBody;
        try
        {
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, leaveOpen: true);
            rawBody = await reader.ReadToEndAsync();
            Request.Body.Position = 0;
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao ler o corpo da requisição: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(rawBody))
            return BadRequest("Corpo da requisição vazio.");

        PostagemDTO? dto;
        try
        {
            dto = System.Text.Json.JsonSerializer.Deserialize<PostagemDTO>(rawBody, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            return BadRequest($"JSON inválido: {ex.Message}");
        }

        if (dto is null)
            return BadRequest("DTO inválido ou ausente.");
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
