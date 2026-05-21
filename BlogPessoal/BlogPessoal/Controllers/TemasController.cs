using BlogPessoal.Data;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Controllers
{
    [Route("api/temas")]
    [ApiController]
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
            var temas = _repository.GetTemas();
            if(temas is null)
            {
                return NotFound("Temas não encontrados!");
            }
            return Ok(temas);
        }

        [HttpGet("{id:int}", Name = "ObterTema")]
        public ActionResult<Tema> Get(int id)
        {
            var temaId = _repository.GetTema(id);
            if (temaId is null)
            {
                return NotFound($"Não foi possível localizar o Tema com id = {id}.");
            }
            return temaId;                
        }

        [HttpPost]
        public ActionResult Post(Tema tema)
        {
            if (tema is null)
            {
                return BadRequest("Falha na criação do tema");
            }
            var temaCriado = _repository.Create(tema);               

            return new CreatedAtRouteResult("ObterTema",
                new { id = temaCriado.TemaID }, temaCriado);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Tema tema)
        {
            if (id != tema.TemaID)
            {
                return BadRequest($"Tema com id = {id} não encontrado");
            }

            _repository.Update(id, tema);
            return Ok(tema);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var temaExcluido = _repository.Delete(id);
            if (temaExcluido is null)
            {
                return NotFound($"Tema com id = {id} não encontrado");
            }
            return Ok(temaExcluido);
        }
    }
}
