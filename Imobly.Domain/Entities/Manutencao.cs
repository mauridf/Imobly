using System.ComponentModel.DataAnnotations;
using Imobly.Domain.Enums;

namespace Imobly.Domain.Entities
{
    public class Manutencao : BaseEntity
    {
        public Guid ImovelId { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required]
        public DateTime Data { get; set; }

        public decimal Valor { get; set; }

        [MaxLength(150)]
        public string Responsavel { get; set; }

        public StatusManutencao Status { get; set; } = StatusManutencao.Pendente;

        // Navigation Properties
        public virtual Imovel Imovel { get; set; }
    }
}