using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Imobly.Domain.Enums;
using Imobly.Domain.ValueObjects;

namespace Imobly.Domain.Entities
{
    public class Imovel : BaseEntity
    {
        // Foreign Key
        public Guid UsuarioId { get; set; }

        // Properties
        public TipoImovel Tipo { get; set; }

        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        // Endereço como propriedades simples (mais fácil para queries)
        public string EnderecoLogradouro { get; set; }
        public string EnderecoNumero { get; set; }
        public string EnderecoComplemento { get; set; }
        public string EnderecoBairro { get; set; }
        public string EnderecoCidade { get; set; }
        public string EnderecoEstado { get; set; }
        public string EnderecoCEP { get; set; }

        public decimal AreaM2 { get; set; }
        public int Quartos { get; set; }
        public int Banheiros { get; set; }
        public int VagasGaragem { get; set; }
        public decimal ValorAluguelSugerido { get; set; }
        public bool Ativo { get; set; } = true;

        // Navigation Properties
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<MidiaImovel> Midias { get; set; } = new List<MidiaImovel>();
        public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
        public virtual ICollection<Manutencao> Manutencoes { get; set; } = new List<Manutencao>();
        public virtual ICollection<Seguro> Seguros { get; set; } = new List<Seguro>();
        public virtual ICollection<MovimentacaoFinanceira> MovimentacoesFinanceiras { get; set; } = new List<MovimentacaoFinanceira>();

        // Métodos
        public void DefinirEndereco(Endereco endereco)
        {
            EnderecoLogradouro = endereco.Logradouro;
            EnderecoNumero = endereco.Numero;
            EnderecoComplemento = endereco.Complemento;
            EnderecoBairro = endereco.Bairro;
            EnderecoCidade = endereco.Cidade;
            EnderecoEstado = endereco.Estado;
            EnderecoCEP = endereco.CEP;
            Atualizar();
        }

        public void Ativar()
        {
            Ativo = true;
            Atualizar();
        }

        public void Desativar()
        {
            Ativo = false;
            Atualizar();
        }

        public Endereco ObterEndereco()
        {
            return new Endereco(
                EnderecoLogradouro,
                EnderecoNumero,
                EnderecoBairro,
                EnderecoCidade,
                EnderecoEstado,
                EnderecoCEP,
                EnderecoComplemento
            );
        }
    }
}