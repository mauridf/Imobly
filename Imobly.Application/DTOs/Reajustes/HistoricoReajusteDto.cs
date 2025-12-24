namespace Imobly.Application.DTOs.Reajustes
{
    public class HistoricoReajusteDto
    {
        public Guid Id { get; set; }
        public Guid ContratoId { get; set; }
        public decimal ValorAnterior { get; set; }
        public decimal ValorNovo { get; set; }
        public DateTime DataReajuste { get; set; }
        public string IndiceUtilizado { get; set; }
        public DateTime CriadoEm { get; set; }
    }

    public class CriarHistoricoReajusteDto
    {
        public Guid ContratoId { get; set; }
        public decimal ValorNovo { get; set; }
        public string IndiceUtilizado { get; set; }
    }
}