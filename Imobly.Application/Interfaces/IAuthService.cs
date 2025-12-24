using Imobly.Application.DTOs.Autenticacao;
using Imobly.Application.DTOs.Usuarios;

namespace Imobly.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<UsuarioDto> RegistrarAsync(RegistrarRequest request);
        Task<bool> AlterarSenhaAsync(Guid usuarioId, AlterarSenhaDto request);
    }
}