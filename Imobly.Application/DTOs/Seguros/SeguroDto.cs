namespace Imobly.Application.DTOs.Seguros
{
    public class SeguroDto
    {
        public Guid Id { get; set; }
        public Guid ImovelId { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Seguradora { get; set; }
        public string Apolice { get; set; }
        public DateTime CriadoEm { get; set; }
        public string ImovelTitulo { get; set; }
    }

    public class CriarSeguroDto
    {
        public Guid ImovelId { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Seguradora { get; set; }
        public string Apolice { get; set; }
    }

    public class AtualizarSeguroDto
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Seguradora { get; set; }
        public string Apolice { get; set; }
    }
}