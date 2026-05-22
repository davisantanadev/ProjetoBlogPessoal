using BlogPessoal.DTOs;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogPessoal.Controllers;

[Route("api/ia")]
[ApiController]
[Authorize]
public class IaController : ControllerBase
{
    private readonly IIaService _iaService;

    public IaController(IIaService iaService)
    {
        _iaService = iaService;
    }

    [HttpPost("resumir")]
    public async Task<ActionResult<IaResponseDTO>> Resumir(
        IaRequestDTO request,
        CancellationToken cancellationToken)
    {
        var resultado = await _iaService.GerarResumoAsync(request, cancellationToken);
        return Ok(resultado);
    }
}
