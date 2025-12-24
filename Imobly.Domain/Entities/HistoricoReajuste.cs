using System.ComponentModel.DataAnnotations;

namespace Imobly.Domain.Entities
{
    public class HistoricoReajuste : BaseEntity
    {
        public Guid ContratoId { get; set; }

        [Required]
        public decimal ValorAnterior { get; set; }

        [Required]
        public decimal ValorNovo { get; set; }

        [Required]
        public DateTime DataReajuste { get; set; }

        [MaxLength(50)]
        public string IndiceUtilizado { get; set; } // INCC, IPCA etc.

        // Navigation Properties
        public virtual Contrato Contrato { get; set; }
    }
}