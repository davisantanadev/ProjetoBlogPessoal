using BlogPessoal.Data;
using BlogPessoal.DTOs;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;

        public UsuariosController(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("cadastrar")]
        public ActionResult Post(UsuarioDTO dto)
        {
            var usuario = dto.ToEntity();
            _repository.Create(usuario);
            return Ok(usuario.ToResponseDTO());
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, UsuarioDTO dto)
        {
            var usuario = dto.ToEntity();
            usuario.UsuarioId = id;
            _repository.Update(id, usuario);
            return Ok(usuario.ToResponseDTO());
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var usuario = _repository.Delete(id);
            if (usuario is null)
                return NotFound($"Usuário com id = {id} não encontrado");
            return Ok(usuario.ToResponseDTO());
        }
    }
}
