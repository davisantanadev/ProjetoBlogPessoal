using BlogPessoal.Models;

namespace BlogPessoal.DTOs;

public static class MappingExtensions
{
    public static Usuario ToEntity(this UsuarioDTO dto)
    {
        return new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = dto.Senha,
            FotoURL = dto.FotoURL
        };
    }

    public static UsuarioResponseDTO ToResponseDTO(this Usuario usuario)
    {
        return new UsuarioResponseDTO
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Email = usuario.Email,
            FotoURL = usuario.FotoURL
        };
    }

    public static Postagem ToEntity(this PostagemDTO dto, Tema tema, Usuario usuario)
    {
        return new Postagem
        {
            Titulo = dto.Titulo,
            Texto = dto.Texto,
            ImagemURL = dto.ImagemURL,
            Data = dto.Data,
            Tema = tema,
            Usuario = usuario
        };
    }
}
