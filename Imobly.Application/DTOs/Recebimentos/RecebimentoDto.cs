namespace Imobly.Application.DTOs.Recebimentos
{
    public class RecebimentoDto
    {
        public Guid Id { get; set; }
        public Guid ContratoId { get; set; }
        public DateTime Competencia { get; set; }
        public decimal ValorPrevisto { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string Status { get; set; }
        public DateTime CriadoEm { get; set; }
        public string ImovelTitulo { get; set; }
        public string LocatarioNome { get; set; }
    }

    public class CriarRecebimentoDto
    {
        public Guid ContratoId { get; set; }
        public DateTime Competencia { get; set; }
        public decimal ValorPrevisto { get; set; }
    }

    public class RegistrarPagamentoDto
    {
        public decimal ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
    }

    public class GerarRecebimentosDto
    {
        public Guid ContratoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public decimal ValorAluguel { get; set; }
        public int DiaVencimento { get; set; }
    }
}