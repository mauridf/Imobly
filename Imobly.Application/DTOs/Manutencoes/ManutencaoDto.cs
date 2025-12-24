namespace Imobly.Application.DTOs.Manutencoes
{
    public class ManutencaoDto
    {
        public Guid Id { get; set; }
        public Guid ImovelId { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Responsavel { get; set; }
        public string Status { get; set; }
        public DateTime CriadoEm { get; set; }
        public string ImovelTitulo { get; set; }
    }

    public class CriarManutencaoDto
    {
        public Guid ImovelId { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Responsavel { get; set; }
    }

    public class AtualizarManutencaoDto
    {
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Responsavel { get; set; }
        public string Status { get; set; }
    }
}