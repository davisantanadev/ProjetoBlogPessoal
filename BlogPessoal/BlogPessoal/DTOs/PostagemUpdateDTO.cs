using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.DTOs;

public class PostagemUpdateDTO
{
    [StringLength(50)]
    public string? Titulo { get; set; }

    [StringLength(500)]
    public string? Texto { get; set; }

    [StringLength(500)]
    public string? ImagemURL { get; set; }

    public DateTime? Data { get; set; }

    public int? TemaId { get; set; }

    public int? UsuarioId { get; set; }
}
