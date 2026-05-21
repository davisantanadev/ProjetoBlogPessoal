using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPessoal.Models;

[Table("Postagens")]
public class Postagem
{
    [Key]
    public int PostagemId { get; set; }

    [Required]
    [StringLength(50)]
    public string? Titulo { get; set; }

    [Required]
    [StringLength(500)]
    public string? Texto { get; set; }

    [Required]
    [StringLength(500)]
    public string? ImagemURL { get; set; }
    public DateTime Data { get; set; }

    [StringLength(500)]
    public string? ResumoIA { get; set; }
    [StringLength(50)]
    public string? TagIA { get; set; }
    [StringLength(50)]
    public string? CategoriaIA { get; set; }
    public Tema? Tema { get; set; }
    public Usuario? Usuario { get; set; }
}
 