namespace BlogPessoal.Models;

public class UsuarioLogin
{
    public int UsuarioId { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public string? FotoURL { get; set; }
    public string? Token { get; set; }
}
