using BlogPessoal.DTOs;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogPessoal.Controllers;

[Route("api/usuarios")]
[ApiController]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuariosController(
        IUsuarioRepository repository,
        ITokenService tokenService,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _repository = repository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("cadastrar")]
    [AllowAnonymous]
    public ActionResult Post(UsuarioDTO dto)
    {
        if (dto.Email is null || dto.Senha is null)
            return BadRequest("Email e senha sao obrigatorios.");

        var usuarioExistente = _repository.GetUsuarioByEmail(dto.Email);
        if (usuarioExistente is not null)
            return Conflict("Email ja cadastrado.");

        var usuario = dto.ToEntity();
        usuario.Senha = _passwordHasher.HashPassword(usuario, dto.Senha);
        _repository.Create(usuario);

        return Ok(usuario.ToResponseDTO());
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<UsuarioLogin> Login(UsuarioLogin usuarioLogin)
    {
        if (usuarioLogin.Email is null || usuarioLogin.Senha is null)
            return BadRequest("Email e senha sao obrigatorios.");

        var usuario = _repository.GetUsuarioByEmail(usuarioLogin.Email);
        if (usuario is null || !SenhaValida(usuario, usuarioLogin.Senha))
            return Unauthorized("Email ou senha invalidos.");

        if (SenhaEmTextoPlanoLegada(usuario, usuarioLogin.Senha))
        {
            usuario.Senha = _passwordHasher.HashPassword(usuario, usuarioLogin.Senha);
            _repository.Update(usuario.UsuarioId, usuario);
        }

        usuarioLogin.UsuarioId = usuario.UsuarioId;
        usuarioLogin.Nome = usuario.Nome;
        usuarioLogin.Email = usuario.Email;
        usuarioLogin.Senha = null;
        usuarioLogin.FotoURL = usuario.FotoURL;
        usuarioLogin.Token = _tokenService.GenerateToken(usuario);

        return Ok(usuarioLogin);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, [FromBody] UsuarioDTO dto)
    {
        if (dto.Senha is null)
            return BadRequest("Senha e obrigatoria.");

        var usuarioExistente = _repository.GetUsuario(id);
        if (usuarioExistente is null)
            return NotFound($"Usuário com id = {id} não encontrado");

        if (dto.Email is not null)
        {
            var usuarioComMesmoEmail = _repository.GetUsuarioByEmail(dto.Email);
            if (usuarioComMesmoEmail is not null && usuarioComMesmoEmail.UsuarioId != id)
                return Conflict("Email ja cadastrado.");
        }

        var usuario = dto.ToEntity();
        usuario.UsuarioId = id;
        usuario.Senha = _passwordHasher.HashPassword(usuario, dto.Senha);
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

    private bool SenhaValida(Usuario usuario, string senha)
    {
        if (string.IsNullOrWhiteSpace(usuario.Senha))
            return false;

        try
        {
            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
            if (resultado != PasswordVerificationResult.Failed)
                return true;
        }
        catch (FormatException)
        {
            return SenhaEmTextoPlanoLegada(usuario, senha);
        }

        return SenhaEmTextoPlanoLegada(usuario, senha);
    }

    private static bool SenhaEmTextoPlanoLegada(Usuario usuario, string senha)
    {
        return usuario.Senha == senha;
    }
}
