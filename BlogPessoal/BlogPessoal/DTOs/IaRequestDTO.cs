using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.DTOs;

public class IaRequestDTO
{
    [Required]
    [StringLength(50)]
    public string? Titulo { get; set; }

    [Required]
    [StringLength(500)]
    public string? Texto { get; set; }
}
