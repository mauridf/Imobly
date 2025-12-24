using System.ComponentModel.DataAnnotations;
using Imobly.Domain.Enums;

namespace Imobly.Domain.Entities
{
    public class MovimentacaoFinanceira : BaseEntity
    {
        public Guid? ImovelId { get; set; }

        [Required]
        public TipoMovimentacao Tipo { get; set; }

        [Required]
        public CategoriaMovimentacao Categoria { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public DateTime Data { get; set; }

        public StatusMovimentacao Status { get; set; } = StatusMovimentacao.Pendente;

        // Navigation Properties
        public virtual Imovel Imovel { get; set; }
    }
}