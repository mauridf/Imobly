using System.ComponentModel.DataAnnotations;
using Imobly.Domain.Enums;

namespace Imobly.Domain.Entities
{
    public class Contrato : BaseEntity
    {
        public Guid ImovelId { get; set; }
        public Guid LocatarioId { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        [Required]
        public decimal ValorAluguel { get; set; }

        public decimal ValorSeguro { get; set; }
        public int DiaVencimento { get; set; }
        public StatusContrato Status { get; set; } = StatusContrato.Ativo;

        [MaxLength(255)]
        public string CaminhoDocumentoPDF { get; set; }

        // Navigation Properties
        public virtual Imovel Imovel { get; set; }
        public virtual Locatario Locatario { get; set; }
        public virtual ICollection<Recebimento> Recebimentos { get; set; } = new List<Recebimento>();
        public virtual ICollection<HistoricoReajuste> HistoricosReajuste { get; set; } = new List<HistoricoReajuste>();

        // Métodos
        public bool EstaAtivo()
        {
            return Status == StatusContrato.Ativo &&
                   DataInicio <= DateTime.UtcNow &&
                   DataFim >= DateTime.UtcNow;
        }

        public void Encerrar()
        {
            Status = StatusContrato.Encerrado;
            Atualizar();
        }

        public void Suspender()
        {
            Status = StatusContrato.Suspenso;
            Atualizar();
        }

        public void Reativar()
        {
            Status = StatusContrato.Ativo;
            Atualizar();
        }

        public int ObterMesesDuracao()
        {
            return ((DataFim.Year - DataInicio.Year) * 12) + DataFim.Month - DataInicio.Month + 1;
        }
    }
}