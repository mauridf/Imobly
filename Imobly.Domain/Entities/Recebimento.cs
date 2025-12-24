using System.ComponentModel.DataAnnotations;
using Imobly.Domain.Enums;

namespace Imobly.Domain.Entities
{
    public class Recebimento : BaseEntity
    {
        public Guid ContratoId { get; set; }

        [Required]
        public DateTime Competencia { get; set; } // Ex: 2025-02-01

        [Required]
        public decimal ValorPrevisto { get; set; }

        public decimal ValorPago { get; set; }
        public DateTime? DataPagamento { get; set; }
        public StatusRecebimento Status { get; set; } = StatusRecebimento.Aguardando;

        // Navigation Properties
        public virtual Contrato Contrato { get; set; }

        // Métodos
        public void RegistrarPagamento(decimal valorPago, DateTime dataPagamento)
        {
            ValorPago = valorPago;
            DataPagamento = dataPagamento;

            // Determinar status baseado na data
            var hoje = DateTime.UtcNow;
            var vencimento = new DateTime(Competencia.Year, Competencia.Month,
                                         Contrato?.DiaVencimento ?? 10);

            if (dataPagamento < vencimento)
                Status = StatusRecebimento.Adiantado;
            else if (dataPagamento <= vencimento.AddDays(5))
                Status = StatusRecebimento.Pago;
            else
                Status = StatusRecebimento.Atrasado;

            Atualizar();
        }

        public bool EstaVencido()
        {
            var hoje = DateTime.UtcNow;
            var vencimento = new DateTime(Competencia.Year, Competencia.Month,
                                         Contrato?.DiaVencimento ?? 10);

            return Status == StatusRecebimento.Aguardando && hoje > vencimento.AddDays(5);
        }
    }
}