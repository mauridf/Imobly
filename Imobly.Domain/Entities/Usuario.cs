using System.ComponentModel.DataAnnotations;

namespace Imobly.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string SenhaHash { get; set; }

        [MaxLength(20)]
        public string Telefone { get; set; }

        // Navigation Properties
        public virtual ICollection<Imovel> Imoveis { get; set; } = new List<Imovel>();

        // Métodos
        public void AtualizarSenha(string novaSenhaHash)
        {
            SenhaHash = novaSenhaHash;
            Atualizar();
        }

        public void AtualizarContato(string telefone)
        {
            Telefone = telefone;
            Atualizar();
        }
    }
}