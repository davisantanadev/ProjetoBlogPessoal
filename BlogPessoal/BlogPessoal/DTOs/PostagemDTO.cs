using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.DTOs;

public class PostagemDTO
{
    [Required]
    [StringLength(50)]
    public string? Titulo { get; set; }

    [Required]
    [StringLength(500)]
    public string? Texto { get; set; }

    [StringLength(500)]
    public string? ImagemURL { get; set; }

    public DateTime Data { get; set; }

    [Required]
    public int TemaId { get; set; }

    [Required]
    public int UsuarioId { get; set; }
}
