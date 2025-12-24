using System.ComponentModel.DataAnnotations;

namespace Imobly.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CriadoEm = DateTime.UtcNow;
        }

        public void Atualizar()
        {
            AtualizadoEm = DateTime.UtcNow;
        }
    }
}