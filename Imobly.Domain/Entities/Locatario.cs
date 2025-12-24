using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Imobly.Domain.Enums;

namespace Imobly.Domain.Entities
{
    public class Locatario : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Telefone { get; set; }

        [Required]
        [MaxLength(14)]
        public string CPF { get; set; }

        [MaxLength(30)]
        public string RG { get; set; }

        public DateTime? DataNascimento { get; set; }

        // Endereço
        public string EnderecoLogradouro { get; set; }
        public string EnderecoNumero { get; set; }
        public string EnderecoBairro { get; set; }
        public string EnderecoCidade { get; set; }
        public string EnderecoEstado { get; set; }
        public string EnderecoCEP { get; set; }

        public StatusLocatario Status { get; set; } = StatusLocatario.Adimplente;

        // Navigation Properties
        public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

        // Métodos
        public void MarcarComoInadimplente()
        {
            Status = StatusLocatario.Inadimplente;
            Atualizar();
        }

        public void MarcarComoAdimplente()
        {
            Status = StatusLocatario.Adimplente;
            Atualizar();
        }
    }
}