using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BlogPessoal.Config;
using BlogPessoal.DTOs;
using Microsoft.Extensions.Options;

namespace BlogPessoal.Services;

public class IaService : IIaService
{
    private readonly HttpClient _httpClient;
    private readonly AiOptions _options;
    private readonly ILogger<IaService> _logger;

    public IaService(
        HttpClient httpClient,
        IOptions<AiOptions> options,
        ILogger<IaService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IaResponseDTO> GerarResumoAsync(
        IaRequestDTO request,
        CancellationToken cancellationToken = default)
    {
        if (UsarModoLocal())
            return GerarResumoLocal(request);

        try
        {
            return await GerarResumoExternoAsync(request, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(exception, "Falha ao consultar o servico de IA. Usando resposta local.");
            return GerarResumoLocal(request);
        }
    }

    private bool UsarModoLocal()
    {
        return string.Equals(_options.Provider, "Local", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(_options.Endpoint)
            || string.IsNullOrWhiteSpace(_options.ApiKey)
            || string.IsNullOrWhiteSpace(_options.Model);
    }

    private async Task<IaResponseDTO> GerarResumoExternoAsync(
        IaRequestDTO request,
        CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        httpRequest.Content = JsonContent.Create(new
        {
            model = _options.Model,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "Responda somente JSON valido com as propriedades resumo, tags e categoria."
                },
                new
                {
                    role = "user",
                    content = $"Titulo: {request.Titulo}\nTexto: {request.Texto}"
                }
            }
        });

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);

        var content = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
            return GerarResumoLocal(request);

        var resultado = JsonSerializer.Deserialize<IaResponseDTO>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return resultado is null ? GerarResumoLocal(request) : NormalizarResposta(resultado);
    }

    private IaResponseDTO GerarResumoLocal(IaRequestDTO request)
    {
        var texto = request.Texto?.Trim() ?? string.Empty;
        var resumo = texto.Length <= _options.MaxSummaryLength
            ? texto
            : string.Concat(texto.AsSpan(0, _options.MaxSummaryLength), "...");

        return NormalizarResposta(new IaResponseDTO
        {
            Resumo = resumo,
            Tags = GerarTags(texto),
            Categoria = DefinirCategoria(request.Titulo, texto)
        });
    }

    private static string GerarTags(string texto)
    {
        var palavras = texto
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(palavra => palavra.Trim('.', ',', ';', ':', '!', '?', '"', '\'').ToLowerInvariant())
            .Where(palavra => palavra.Length > 4)
            .Distinct()
            .Take(5);

        return string.Join(", ", palavras);
    }

    private static string DefinirCategoria(string? titulo, string texto)
    {
        var conteudo = $"{titulo} {texto}".ToLowerInvariant();

        if (conteudo.Contains("tecnologia") || conteudo.Contains("programacao") || conteudo.Contains("software"))
            return "Tecnologia";

        if (conteudo.Contains("saude") || conteudo.Contains("bem-estar"))
            return "Saude";

        if (conteudo.Contains("educacao") || conteudo.Contains("estudo") || conteudo.Contains("aprendizado"))
            return "Educacao";

        return "Geral";
    }

    private static string LimitarTexto(string texto, int tamanhoMaximo)
    {
        return texto.Length <= tamanhoMaximo ? texto : texto[..tamanhoMaximo];
    }

    private static IaResponseDTO NormalizarResposta(IaResponseDTO response)
    {
        response.Resumo = string.IsNullOrWhiteSpace(response.Resumo)
            ? null
            : LimitarTexto(response.Resumo, 500);
        response.Tags = string.IsNullOrWhiteSpace(response.Tags)
            ? null
            : LimitarTexto(response.Tags, 50);
        response.Categoria = string.IsNullOrWhiteSpace(response.Categoria)
            ? "Geral"
            : LimitarTexto(response.Categoria, 50);

        return response;
    }
}
