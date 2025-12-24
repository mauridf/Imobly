using Imobly.Application.DTOs.Usuarios;

namespace Imobly.Application.DTOs.Autenticacao
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ExpiraEm { get; set; }
        public UsuarioDto Usuario { get; set; }
    }

    public class RegistrarRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string ConfirmarSenha { get; set; }
        public string Telefone { get; set; }
    }
}