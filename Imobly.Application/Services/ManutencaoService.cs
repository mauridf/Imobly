using AutoMapper;
using Imobly.Application.DTOs.Manutencoes;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class ManutencaoService : IManutencaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ManutencaoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ManutencaoDto> GetByIdAsync(Guid id, Guid usuarioId)
        {
            var manutencao = await _unitOfWork.Manutencoes.GetByIdAsync(id);
            if (manutencao == null)
            {
                throw new KeyNotFoundException("Manutenção não encontrada");
            }

            // Carregar imóvel para validação
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(manutencao.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            return _mapper.Map<ManutencaoDto>(manutencao);
        }

        public async Task<IEnumerable<ManutencaoDto>> GetByImovelAsync(Guid imovelId, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(imovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            var manutencoes = await _unitOfWork.Manutencoes.FindAsync(m => m.ImovelId == imovelId);
            return _mapper.Map<IEnumerable<ManutencaoDto>>(manutencoes);
        }

        public async Task<ManutencaoDto> CreateAsync(CriarManutencaoDto dto, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(dto.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            var manutencao = _mapper.Map<Manutencao>(dto);
            await _unitOfWork.Manutencoes.AddAsync(manutencao);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ManutencaoDto>(manutencao);
        }

        public async Task<ManutencaoDto> UpdateAsync(Guid id, AtualizarManutencaoDto dto, Guid usuarioId)
        {
            var manutencao = await _unitOfWork.Manutencoes.GetByIdAsync(id);
            if (manutencao == null)
            {
                throw new KeyNotFoundException("Manutenção não encontrada");
            }

            // Carregar imóvel para validação
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(manutencao.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            _mapper.Map(dto, manutencao);
            manutencao.Atualizar();

            _unitOfWork.Manutencoes.Update(manutencao);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ManutencaoDto>(manutencao);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid usuarioId)
        {
            var manutencao = await _unitOfWork.Manutencoes.GetByIdAsync(id);
            if (manutencao == null)
            {
                throw new KeyNotFoundException("Manutenção não encontrada");
            }

            // Carregar imóvel para validação
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(manutencao.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            _unitOfWork.Manutencoes.Remove(manutencao);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> MarcarComoFeitaAsync(Guid id, Guid usuarioId)
        {
            var manutencao = await _unitOfWork.Manutencoes.GetByIdAsync(id);
            if (manutencao == null)
            {
                throw new KeyNotFoundException("Manutenção não encontrada");
            }

            // Carregar imóvel para validação
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(manutencao.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            manutencao.Status = StatusManutencao.Feito;
            manutencao.Atualizar();

            _unitOfWork.Manutencoes.Update(manutencao);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}