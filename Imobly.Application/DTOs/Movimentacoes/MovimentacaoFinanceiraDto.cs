namespace Imobly.Application.DTOs.Movimentacoes
{
    public class MovimentacaoFinanceiraDto
    {
        public Guid Id { get; set; }
        public Guid? ImovelId { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; }
        public DateTime CriadoEm { get; set; }
        public string ImovelTitulo { get; set; }
    }

    public class CriarMovimentacaoFinanceiraDto
    {
        public Guid? ImovelId { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
    }

    public class AtualizarMovimentacaoFinanceiraDto
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; }
    }
}