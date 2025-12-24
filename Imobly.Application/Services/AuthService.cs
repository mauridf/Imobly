using AutoMapper;
using Imobly.Application.DTOs.Autenticacao;
using Imobly.Application.DTOs.Usuarios;
using Imobly.Application.Interfaces;
using Imobly.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Imobly.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            var token = GenerateJwtToken(usuario);
            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

            return new LoginResponse
            {
                Token = token,
                ExpiraEm = DateTime.UtcNow.AddHours(8),
                Usuario = usuarioDto
            };
        }

        public async Task<UsuarioDto> RegistrarAsync(RegistrarRequest request)
        {
            if (request.Senha != request.ConfirmarSenha)
            {
                throw new ArgumentException("As senhas não coincidem");
            }

            if (await _unitOfWork.Usuarios.EmailExistsAsync(request.Email))
            {
                throw new ArgumentException("Email já cadastrado");
            }

            var usuario = new Domain.Entities.Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Telefone = request.Telefone
            };

            await _unitOfWork.Usuarios.AddAsync(usuario);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UsuarioDto>(usuario);
        }

        public async Task<bool> AlterarSenhaAsync(Guid usuarioId, AlterarSenhaDto request)
        {
            if (request.NovaSenha != request.ConfirmarNovaSenha)
            {
                throw new ArgumentException("As novas senhas não coincidem");
            }

            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.SenhaHash))
            {
                throw new UnauthorizedAccessException("Senha atual inválida");
            }

            usuario.AtualizarSenha(BCrypt.Net.BCrypt.HashPassword(request.NovaSenha));
            _unitOfWork.Usuarios.Update(usuario);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private string GenerateJwtToken(Domain.Entities.Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Name, usuario.Nome)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}