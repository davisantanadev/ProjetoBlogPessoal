using BlogPessoal.DTOs;

namespace BlogPessoal.Services;

public interface IIaService
{
    Task<IaResponseDTO> GerarResumoAsync(IaRequestDTO request, CancellationToken cancellationToken = default);
}
