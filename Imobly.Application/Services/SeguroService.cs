using AutoMapper;
using Imobly.Application.DTOs.Seguros;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class SeguroService : ISeguroService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SeguroService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SeguroDto> GetByIdAsync(Guid id, Guid usuarioId)
        {
            var seguro = await _unitOfWork.Seguros.GetByIdAsync(id);
            if (seguro == null)
            {
                throw new KeyNotFoundException("Seguro não encontrado");
            }

            // Carregar imóvel para validação
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(seguro.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            return _mapper.Map<SeguroDto>(seguro);
        }

        public async Task<IEnumerable<SeguroDto>> GetByImovelAsync(Guid imovelId, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(imovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            var seguros = await _unitOfWork.Seguros.FindAsync(s => s.ImovelId == imovelId);
            return _mapper.Map<IEnumerable<SeguroDto>>(seguros);
        }

        public async Task<IEnumerable<SeguroDto>> GetVencendoProximos30DiasAsync(Guid usuarioId)
        {
            var hoje = DateTime.UtcNow;
            var dataLimite = hoje.AddDays(30);

            // Buscar todos os seguros dos imóveis do usuário
            var imoveis = await _unitOfWork.Imoveis.GetByUsuarioIdAsync(usuarioId);
            var imoveisIds = imoveis.Select(i => i.Id);

            var seguros = await _unitOfWork.Seguros.FindAsync(s =>
                imoveisIds.Contains(s.ImovelId) &&
                s.DataFim >= hoje &&
                s.DataFim <= dataLimite);

            return _mapper.Map<IEnumerable<SeguroDto>>(seguros);
        }

        public async Task<SeguroDto> CreateAsync(CriarSeguroDto dto, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(dto.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            // Validar datas
            if (dto.DataInicio >= dto.DataFim)
            {
                throw new ArgumentException("Data de início deve ser anterior à data de fim");
            }

            var seguro = _mapper.Map<Seguro>(dto);
            await _unitOfWork.Seguros.AddAsync(seguro);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SeguroDto>(seguro);
        }

        public async Task<SeguroDto> UpdateAsync(Guid id, AtualizarSeguroDto dto, Guid usuarioId)
        {
            var seguro = await _unitOfWork.Seguros.GetByIdAsync(id);
            if (seguro == null)
            {
                throw new KeyNotFoundException("Seguro não encontrado");
            }

            // Carregar imóvel para validação
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(seguro.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            // Validar datas
            if (dto.DataInicio >= dto.DataFim)
            {
                throw new ArgumentException("Data de início deve ser anterior à data de fim");
            }

            _mapper.Map(dto, seguro);
            seguro.Atualizar();

            _unitOfWork.Seguros.Update(seguro);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SeguroDto>(seguro);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid usuarioId)
        {
            var seguro = await _unitOfWork.Seguros.GetByIdAsync(id);
            if (seguro == null)
            {
                throw new KeyNotFoundException("Seguro não encontrado");
            }

            // Carregar imóvel para validação
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(seguro.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            _unitOfWork.Seguros.Remove(seguro);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<SeguroDto>> SearchAsync(string seguradora, string apolice, Guid usuarioId)
        {
            var imoveis = await _unitOfWork.Imoveis.GetByUsuarioIdAsync(usuarioId);
            var imoveisIds = imoveis.Select(i => i.Id);

            var query = _unitOfWork.Seguros.FindAsync(s => imoveisIds.Contains(s.ImovelId));

            if (!string.IsNullOrWhiteSpace(seguradora))
            {
                var seguros = await query;
                seguros = seguros.Where(s => s.Seguradora.Contains(seguradora, StringComparison.OrdinalIgnoreCase)).ToList();
                return _mapper.Map<IEnumerable<SeguroDto>>(seguros);
            }

            if (!string.IsNullOrWhiteSpace(apolice))
            {
                var seguros = await query;
                seguros = seguros.Where(s => s.Apolice.Contains(apolice, StringComparison.OrdinalIgnoreCase)).ToList();
                return _mapper.Map<IEnumerable<SeguroDto>>(seguros);
            }

            var todosSeguros = await query;
            return _mapper.Map<IEnumerable<SeguroDto>>(todosSeguros);
        }
    }
}