namespace Imobly.Application.DTOs.Dashboard
{
    public class DashboardResumoDto
    {
        public int TotalImoveis { get; set; }
        public int ImoveisAtivos { get; set; }
        public int TotalLocatarios { get; set; }
        public int ContratosAtivos { get; set; }
        public int RecebimentosPendentes { get; set; }
        public int ManutencoesPendentes { get; set; }
        public decimal ReceitaMensal { get; set; }
        public decimal DespesaMensal { get; set; }
        public decimal SaldoMensal { get; set; }
    }

    public class GraficoReceitaDespesaDto
    {
        public string Mes { get; set; }
        public decimal Receita { get; set; }
        public decimal Despesa { get; set; }
    }

    public class ContratoProximoVencimentoDto
    {
        public Guid Id { get; set; }
        public string ImovelTitulo { get; set; }
        public string LocatarioNome { get; set; }
        public DateTime DataFim { get; set; }
        public int DiasParaVencimento { get; set; }
    }

    public class ManutencaoPendenteDto
    {
        public Guid Id { get; set; }
        public string ImovelTitulo { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
    }
}