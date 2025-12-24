using Imobly.Application.DTOs.Reajustes;

namespace Imobly.Application.DTOs.Contratos
{
    public class ContratoDto
    {
        public Guid Id { get; set; }
        public Guid ImovelId { get; set; }
        public Guid LocatarioId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public decimal ValorAluguel { get; set; }
        public decimal ValorSeguro { get; set; }
        public int DiaVencimento { get; set; }
        public string Status { get; set; }
        public string CaminhoDocumentoPDF { get; set; }
        public DateTime CriadoEm { get; set; }
        public string ImovelTitulo { get; set; }
        public string LocatarioNome { get; set; }
    }

    public class CriarContratoDto
    {
        public Guid ImovelId { get; set; }
        public Guid LocatarioId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public decimal ValorAluguel { get; set; }
        public decimal ValorSeguro { get; set; }
        public int DiaVencimento { get; set; }
    }

    public class AtualizarContratoDto
    {
        public DateTime? DataFim { get; set; }
        public decimal? ValorAluguel { get; set; }
        public decimal? ValorSeguro { get; set; }
        public int? DiaVencimento { get; set; }
        public string Status { get; set; }
    }

    public class ContratoComDetalhesDto : ContratoDto
    {
        public int TotalRecebimentos { get; set; }
        public int RecebimentosPagos { get; set; }
        public int RecebimentosAtrasados { get; set; }
        public decimal TotalRecebido { get; set; }
        public List<HistoricoReajusteDto> HistoricosReajuste { get; set; }
    }
}