using BlogPessoal.Models;
using BlogPessoal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogPessoal.Controllers;

[Route("api/temas")]
[ApiController]
[Authorize]
public class TemasController : ControllerBase
{
    private readonly ITemaRepository _repository;

    public TemasController(ITemaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Tema>> Get()
    {
        var temas = _repository.GetTemas().ToList();
        return Ok(temas);
    }

    [HttpGet("{id:int}", Name = "ObterTema")]
    public ActionResult<Tema> Get(int id)
    {
        var tema = _repository.GetTema(id);
        if (tema is null)
            return NotFound($"Não foi possível localizar o Tema com id = {id}.");

        return tema;
    }

    [HttpPost]
    public ActionResult Post(Tema tema)
    {
        if (tema.TemaID != 0)
            return BadRequest("O id do tema deve ser gerado pelo banco de dados.");

        var temaCriado = _repository.Create(tema);

        return new CreatedAtRouteResult(
            "ObterTema",
            new { id = temaCriado.TemaID },
            temaCriado);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Tema tema)
    {
        var temaExistente = _repository.GetTema(id);
        if (temaExistente is null)
            return NotFound($"Tema com id = {id} não encontrado");

        tema.TemaID = id;
        _repository.Update(id, tema);
        return Ok(tema);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var temaExcluido = _repository.Delete(id);
        if (temaExcluido is null)
            return NotFound($"Tema com id = {id} não encontrado");

        return Ok(temaExcluido);
    }
}
