using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogPessoal.Models;

[Table("Usuarios")]
public class Usuario
{
    [Key]
    public int UsuarioId { get; set; }

    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }

    [Required]
    [StringLength(50)]
    public string? Email { get; set; }

    [Required]
    [StringLength(20)]
    public string? Senha { get; set; }

    [StringLength(500)]
    public string? FotoURL { get; set; }

    [JsonIgnore]
    public List<Postagem>? Postagens { get; set; }
}