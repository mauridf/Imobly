namespace Imobly.Application.DTOs.Dashboard
{
    public class EstatisticasDetalhadasDto
    {
        public decimal TotalRecebidoAno { get; set; }
        public decimal TotalDespesasAno { get; set; }
        public decimal SaldoAno { get; set; }
        public int LocatariosInadimplentes { get; set; }
        public decimal TotalAtrasado { get; set; }
        public decimal PercentualInadimplencia { get; set; }
        public string MesMaisRentavel { get; set; }
        public int ContratosVencendoProximoMes { get; set; }
        public decimal PrevisaoReceitaProximoMes { get; set; }
    }
}