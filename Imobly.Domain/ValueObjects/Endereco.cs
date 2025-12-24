namespace Imobly.Domain.ValueObjects
{
    public class Endereco
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }

        public Endereco(string logradouro, string numero, string bairro, string cidade, string estado, string cep, string complemento = "")
        {
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            CEP = cep;
            Complemento = complemento;
        }

        public override string ToString()
        {
            return $"{Logradouro}, {Numero} {Complemento} - {Bairro}, {Cidade}/{Estado} - {CEP}";
        }
    }
}