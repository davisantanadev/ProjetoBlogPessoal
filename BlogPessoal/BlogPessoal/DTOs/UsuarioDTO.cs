using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.DTOs;

public class UsuarioDTO
{
    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }

    [Required]
    [StringLength(50)]
    public string? Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string? Senha { get; set; }

    [StringLength(500)]
    public string? FotoURL { get; set; }
}
