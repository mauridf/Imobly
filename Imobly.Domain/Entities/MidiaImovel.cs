using System.ComponentModel.DataAnnotations;
using Imobly.Domain.Enums;

namespace Imobly.Domain.Entities
{
    public class MidiaImovel : BaseEntity
    {
        public Guid ImovelId { get; set; }

        [Required]
        [MaxLength(150)]
        public string CaminhoMidia { get; set; }

        public TipoMidia Tipo { get; set; }

        // Navigation Properties
        public virtual Imovel Imovel { get; set; }
    }
}