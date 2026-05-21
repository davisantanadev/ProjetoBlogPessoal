using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogPessoal.Models;

[Table("Temas")]
public class Tema
{
    [Key]
    public int TemaID { get; set; }

    [Required]
    [StringLength(500)]
    public string? Descricao { get; set; }

    [JsonIgnore]
    public List<Postagem>? Postagens { get; set; }
}
