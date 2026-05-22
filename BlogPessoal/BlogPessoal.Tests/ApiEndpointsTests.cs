using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BlogPessoal.Data;
using BlogPessoal.DTOs;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace BlogPessoal.Tests;

public class ApiEndpointsTests : IClassFixture<BlogPessoalApiFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(BlogPessoalApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task FluxoCompleto_DeveAtenderEscopoDoProjeto()
    {
        var cadastro = await _client.PostAsJsonAsync("/api/usuarios/cadastrar", new
        {
            Nome = "API Test",
            Email = $"api{Guid.NewGuid():N}"[..20] + "@ex.com",
            Senha = "Senha123!",
            FotoURL = "https://example.com/foto.jpg"
        });
        cadastro.EnsureSuccessStatusCode();

        var usuarioCriado = await ReadJsonAsync(cadastro);
        var usuarioId = usuarioCriado.GetProperty("usuarioId").GetInt32();
        var email = usuarioCriado.GetProperty("email").GetString();

        var login = await _client.PostAsJsonAsync("/api/usuarios/login", new
        {
            Email = email,
            Senha = "Senha123!"
        });
        login.EnsureSuccessStatusCode();

        var loginJson = await ReadJsonAsync(login);
        var token = loginJson.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var temaCriado = await _client.PostAsJsonAsync("/api/temas", new
        {
            Descricao = "Tecnologia"
        });
        Assert.Equal(HttpStatusCode.Created, temaCriado.StatusCode);

        var temaJson = await ReadJsonAsync(temaCriado);
        var temaId = temaJson.GetProperty("temaID").GetInt32();

        var temas = await _client.GetAsync("/api/temas");
        temas.EnsureSuccessStatusCode();

        var postagemCriada = await _client.PostAsJsonAsync("/api/postagens", new
        {
            Titulo = "Boas APIs",
            Texto = "Hoje vou explicar boas praticas para APIs REST com ASP.NET Core e Entity Framework.",
            ImagemURL = "https://example.com/api.jpg",
            Data = DateTime.UtcNow,
            TemaId = temaId,
            UsuarioId = usuarioId
        });
        postagemCriada.EnsureSuccessStatusCode();

        var postagemJson = await ReadJsonAsync(postagemCriada);
        var postagemId = postagemJson.GetProperty("postagemId").GetInt32();
        Assert.Equal("Resumo de teste", postagemJson.GetProperty("resumoIA").GetString());

        var filtro = await _client.GetAsync($"/api/postagens/filtro?autor={usuarioId}&tema={temaId}");
        filtro.EnsureSuccessStatusCode();
        var filtroJson = await ReadJsonAsync(filtro);
        Assert.NotEmpty(filtroJson.EnumerateArray());

        var ia = await _client.PostAsJsonAsync("/api/ia/resumir", new
        {
            Titulo = "Boas APIs",
            Texto = "Conteudo para resumo."
        });
        ia.EnsureSuccessStatusCode();

        var postagemAtualizada = await _client.PutAsJsonAsync($"/api/postagens/{postagemId}", new
        {
            Texto = "Texto atualizado sobre APIs REST."
        });
        postagemAtualizada.EnsureSuccessStatusCode();
        var postagemAtualizadaJson = await ReadJsonAsync(postagemAtualizada);
        Assert.Equal("Boas APIs", postagemAtualizadaJson.GetProperty("titulo").GetString());
        Assert.Equal("Texto atualizado sobre APIs REST.", postagemAtualizadaJson.GetProperty("texto").GetString());

        var temaAtualizado = await _client.PutAsJsonAsync($"/api/temas/{temaId}", new
        {
            Descricao = "Tecnologia editada"
        });
        temaAtualizado.EnsureSuccessStatusCode();

        var filtroComTemaInexistente = await _client.GetAsync($"/api/postagens/filtro?autor={usuarioId}&tema=999");
        Assert.Equal(HttpStatusCode.NotFound, filtroComTemaInexistente.StatusCode);

        var filtroComAutorInexistente = await _client.GetAsync($"/api/postagens/filtro?autor=999&tema={temaId}");
        Assert.Equal(HttpStatusCode.NotFound, filtroComAutorInexistente.StatusCode);

        var usuarioAtualizado = await _client.PutAsJsonAsync($"/api/usuarios/{usuarioId}", new
        {
            Nome = "API Test Edit",
            Email = email,
            Senha = "Senha123!",
            FotoURL = "https://example.com/foto-edit.jpg"
        });
        usuarioAtualizado.EnsureSuccessStatusCode();

        var deletePostagem = await _client.DeleteAsync($"/api/postagens/{postagemId}");
        deletePostagem.EnsureSuccessStatusCode();

        var deleteTema = await _client.DeleteAsync($"/api/temas/{temaId}");
        deleteTema.EnsureSuccessStatusCode();

        var deleteUsuario = await _client.DeleteAsync($"/api/usuarios/{usuarioId}");
        deleteUsuario.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task EndpointsProtegidos_DevemExigirTokenJwt()
    {
        var resposta = await _client.GetAsync("/api/temas");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.Clone();
    }
}

public class BlogPessoalApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"BlogPessoalTests-{Guid.NewGuid():N}";
    private const string JwtSecret = "test-secret-key-with-enough-length-for-hmac-sha256";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=BlogPessoalTests;Uid=root;Pwd=tests;",
                ["Jwt:Secret"] = JwtSecret,
                ["Jwt:Issuer"] = "BlogPessoal",
                ["Jwt:Audience"] = "BlogPessoal",
                ["Jwt:ExpiresInMinutes"] = "60",
                ["Ai:Provider"] = "Local"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IIaService>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
            services.AddScoped<IIaService, FakeIaService>();
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidIssuer = "BlogPessoal";
                options.TokenValidationParameters.ValidAudience = "BlogPessoal";
                options.TokenValidationParameters.IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });
    }
}

public class FakeIaService : IIaService
{
    public Task<IaResponseDTO> GerarResumoAsync(
        IaRequestDTO request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new IaResponseDTO
        {
            Resumo = "Resumo de teste",
            Tags = "api, teste",
            Categoria = "Tecnologia"
        });
    }
}
