namespace Imobly.Application.DTOs.Imoveis
{
    public class EnderecoDto
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
    }

    public class ImovelDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public EnderecoDto Endereco { get; set; }
        public decimal AreaM2 { get; set; }
        public int Quartos { get; set; }
        public int Banheiros { get; set; }
        public int VagasGaragem { get; set; }
        public decimal ValorAluguelSugerido { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public Guid UsuarioId { get; set; }
    }

    public class CriarImovelDto
    {
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public EnderecoDto Endereco { get; set; }
        public decimal AreaM2 { get; set; }
        public int Quartos { get; set; }
        public int Banheiros { get; set; }
        public int VagasGaragem { get; set; }
        public decimal ValorAluguelSugerido { get; set; }
    }

    public class AtualizarImovelDto
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public EnderecoDto Endereco { get; set; }
        public decimal AreaM2 { get; set; }
        public int Quartos { get; set; }
        public int Banheiros { get; set; }
        public int VagasGaragem { get; set; }
        public decimal ValorAluguelSugerido { get; set; }
        public bool Ativo { get; set; }
    }

    public class ImovelComDetalhesDto : ImovelDto
    {
        public int TotalContratos { get; set; }
        public int ContratosAtivos { get; set; }
        public int ManutencoesPendentes { get; set; }
        public decimal ReceitaMensal { get; set; }
    }
}