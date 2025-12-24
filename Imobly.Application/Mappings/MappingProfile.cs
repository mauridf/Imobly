using AutoMapper;
using Imobly.Application.DTOs.Autenticacao;
using Imobly.Application.DTOs.Contratos;
using Imobly.Application.DTOs.Imoveis;
using Imobly.Application.DTOs.Locatarios;
using Imobly.Application.DTOs.Manutencoes;
using Imobly.Application.DTOs.Movimentacoes;
using Imobly.Application.DTOs.Reajustes;
using Imobly.Application.DTOs.Recebimentos;
using Imobly.Application.DTOs.Seguros;
using Imobly.Application.DTOs.Usuarios;
using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.ValueObjects;

namespace Imobly.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Usuario
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<CriarUsuarioDto, Usuario>()
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore());
            CreateMap<AtualizarUsuarioDto, Usuario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Imoveis, opt => opt.Ignore());

            // Imovel
            CreateMap<Imovel, ImovelDto>()
                .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src =>
                    new EnderecoDto
                    {
                        Logradouro = src.EnderecoLogradouro,
                        Numero = src.EnderecoNumero,
                        Complemento = src.EnderecoComplemento,
                        Bairro = src.EnderecoBairro,
                        Cidade = src.EnderecoCidade,
                        Estado = src.EnderecoEstado,
                        CEP = src.EnderecoCEP
                    }))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()));

            CreateMap<CriarImovelDto, Imovel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.Midias, opt => opt.Ignore())
                .ForMember(dest => dest.Contratos, opt => opt.Ignore())
                .ForMember(dest => dest.Manutencoes, opt => opt.Ignore())
                .ForMember(dest => dest.Seguros, opt => opt.Ignore())
                .ForMember(dest => dest.MovimentacoesFinanceiras, opt => opt.Ignore())
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src =>
                    Enum.Parse<TipoImovel>(src.Tipo)))
                .ForMember(dest => dest.EnderecoLogradouro, opt => opt.MapFrom(src => src.Endereco.Logradouro))
                .ForMember(dest => dest.EnderecoNumero, opt => opt.MapFrom(src => src.Endereco.Numero))
                .ForMember(dest => dest.EnderecoComplemento, opt => opt.MapFrom(src => src.Endereco.Complemento))
                .ForMember(dest => dest.EnderecoBairro, opt => opt.MapFrom(src => src.Endereco.Bairro))
                .ForMember(dest => dest.EnderecoCidade, opt => opt.MapFrom(src => src.Endereco.Cidade))
                .ForMember(dest => dest.EnderecoEstado, opt => opt.MapFrom(src => src.Endereco.Estado))
                .ForMember(dest => dest.EnderecoCEP, opt => opt.MapFrom(src => src.Endereco.CEP));

            CreateMap<AtualizarImovelDto, Imovel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.Tipo, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.Midias, opt => opt.Ignore())
                .ForMember(dest => dest.Contratos, opt => opt.Ignore())
                .ForMember(dest => dest.Manutencoes, opt => opt.Ignore())
                .ForMember(dest => dest.Seguros, opt => opt.Ignore())
                .ForMember(dest => dest.MovimentacoesFinanceiras, opt => opt.Ignore())
                .ForMember(dest => dest.EnderecoLogradouro, opt => opt.MapFrom(src => src.Endereco.Logradouro))
                .ForMember(dest => dest.EnderecoNumero, opt => opt.MapFrom(src => src.Endereco.Numero))
                .ForMember(dest => dest.EnderecoComplemento, opt => opt.MapFrom(src => src.Endereco.Complemento))
                .ForMember(dest => dest.EnderecoBairro, opt => opt.MapFrom(src => src.Endereco.Bairro))
                .ForMember(dest => dest.EnderecoCidade, opt => opt.MapFrom(src => src.Endereco.Cidade))
                .ForMember(dest => dest.EnderecoEstado, opt => opt.MapFrom(src => src.Endereco.Estado))
                .ForMember(dest => dest.EnderecoCEP, opt => opt.MapFrom(src => src.Endereco.CEP));

            // Locatario
            CreateMap<Locatario, LocatarioDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CriarLocatarioDto, Locatario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Contratos, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<AtualizarLocatarioDto, Locatario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CPF, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Contratos, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.EnderecoLogradouro, opt => opt.Ignore())
                .ForMember(dest => dest.EnderecoNumero, opt => opt.Ignore())
                .ForMember(dest => dest.EnderecoBairro, opt => opt.Ignore())
                .ForMember(dest => dest.EnderecoCidade, opt => opt.Ignore())
                .ForMember(dest => dest.EnderecoEstado, opt => opt.Ignore())
                .ForMember(dest => dest.EnderecoCEP, opt => opt.Ignore());

            // Contrato
            CreateMap<Contrato, ContratoDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ImovelTitulo, opt => opt.MapFrom(src => src.Imovel.Titulo))
                .ForMember(dest => dest.LocatarioNome, opt => opt.MapFrom(src => src.Locatario.Nome));

            CreateMap<CriarContratoDto, Contrato>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Imovel, opt => opt.Ignore())
                .ForMember(dest => dest.Locatario, opt => opt.Ignore())
                .ForMember(dest => dest.Recebimentos, opt => opt.Ignore())
                .ForMember(dest => dest.HistoricosReajuste, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CaminhoDocumentoPDF, opt => opt.Ignore());

            // Recebimento
            CreateMap<Recebimento, RecebimentoDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ImovelTitulo, opt => opt.MapFrom(src => src.Contrato.Imovel.Titulo))
                .ForMember(dest => dest.LocatarioNome, opt => opt.MapFrom(src => src.Contrato.Locatario.Nome));

            CreateMap<CriarRecebimentoDto, Recebimento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Contrato, opt => opt.Ignore())
                .ForMember(dest => dest.ValorPago, opt => opt.Ignore())
                .ForMember(dest => dest.DataPagamento, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            // MovimentacaoFinanceira
            CreateMap<MovimentacaoFinanceira, MovimentacaoFinanceiraDto>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ImovelTitulo, opt => opt.MapFrom(src => src.Imovel.Titulo));

            CreateMap<CriarMovimentacaoFinanceiraDto, MovimentacaoFinanceira>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Imovel, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src =>
                    Enum.Parse<TipoMovimentacao>(src.Tipo)))
                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src =>
                    Enum.Parse<CategoriaMovimentacao>(src.Categoria)));

            // Manutencao
            CreateMap<Manutencao, ManutencaoDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ImovelTitulo, opt => opt.MapFrom(src => src.Imovel.Titulo));

            CreateMap<CriarManutencaoDto, Manutencao>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Imovel, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<AtualizarManutencaoDto, Manutencao>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImovelId, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Imovel, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    Enum.Parse<StatusManutencao>(src.Status)));

            // Seguro
            CreateMap<Seguro, SeguroDto>()
                .ForMember(dest => dest.ImovelTitulo, opt => opt.MapFrom(src => src.Imovel.Titulo));

            CreateMap<CriarSeguroDto, Seguro>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Imovel, opt => opt.Ignore());

            CreateMap<AtualizarSeguroDto, Seguro>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImovelId, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Imovel, opt => opt.Ignore());

            // HistoricoReajuste
            CreateMap<HistoricoReajuste, HistoricoReajusteDto>();

            CreateMap<CriarHistoricoReajusteDto, HistoricoReajuste>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ValorAnterior, opt => opt.Ignore())
                .ForMember(dest => dest.DataReajuste, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.Contrato, opt => opt.Ignore());
        }
    }
}