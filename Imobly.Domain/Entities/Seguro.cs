using System.ComponentModel.DataAnnotations;

namespace Imobly.Domain.Entities
{
    public class Seguro : BaseEntity
    {
        public Guid ImovelId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Descricao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        [MaxLength(150)]
        public string Seguradora { get; set; }

        [MaxLength(100)]
        public string Apolice { get; set; }

        // Navigation Properties
        public virtual Imovel Imovel { get; set; }
    }
}