using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogPessoal.Models;
using Microsoft.IdentityModel.Tokens;

namespace BlogPessoal.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Usuario usuario)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("A chave Jwt:Secret nao foi configurada.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiresInMinutes = _configuration.GetValue("Jwt:ExpiresInMinutes", 60);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new(ClaimTypes.Name, usuario.Nome ?? string.Empty),
            new(ClaimTypes.Email, usuario.Email ?? string.Empty)
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
