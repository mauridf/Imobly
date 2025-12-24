using AutoMapper;
using Imobly.Application.DTOs.Imoveis;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;
using Imobly.Domain.ValueObjects;

namespace Imobly.Application.Services
{
    public class ImovelService : IImovelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ImovelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ImovelDto> GetByIdAsync(Guid id, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetWithDetailsAsync(id);

            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            return _mapper.Map<ImovelDto>(imovel);
        }

        public async Task<IEnumerable<ImovelDto>> GetAllByUsuarioAsync(Guid usuarioId)
        {
            var imoveis = await _unitOfWork.Imoveis.GetByUsuarioIdAsync(usuarioId);
            return _mapper.Map<IEnumerable<ImovelDto>>(imoveis);
        }

        public async Task<ImovelDto> CreateAsync(CriarImovelDto dto, Guid usuarioId)
        {
            var imovel = _mapper.Map<Imovel>(dto);
            imovel.UsuarioId = usuarioId;

            await _unitOfWork.Imoveis.AddAsync(imovel);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ImovelDto>(imovel);
        }

        public async Task<ImovelDto> UpdateAsync(Guid id, AtualizarImovelDto dto, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(id);

            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            _mapper.Map(dto, imovel);
            imovel.Atualizar();

            _unitOfWork.Imoveis.Update(imovel);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ImovelDto>(imovel);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(id);

            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            // Verificar se há contratos ativos
            var temContratosAtivos = await _unitOfWork.Contratos.HasContratoAtivoAsync(id);
            if (temContratosAtivos)
            {
                throw new InvalidOperationException("Não é possível excluir imóvel com contratos ativos");
            }

            _unitOfWork.Imoveis.Remove(imovel);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> AtivarAsync(Guid id, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(id);

            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            imovel.Ativar();
            _unitOfWork.Imoveis.Update(imovel);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DesativarAsync(Guid id, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(id);

            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            // Verificar se há contratos ativos
            var temContratosAtivos = await _unitOfWork.Contratos.HasContratoAtivoAsync(id);
            if (temContratosAtivos)
            {
                throw new InvalidOperationException("Não é possível desativar imóvel com contratos ativos");
            }

            imovel.Desativar();
            _unitOfWork.Imoveis.Update(imovel);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<ImovelDto>> SearchAsync(string searchTerm, Guid usuarioId)
        {
            var imoveis = await _unitOfWork.Imoveis.SearchAsync(searchTerm, usuarioId);
            return _mapper.Map<IEnumerable<ImovelDto>>(imoveis);
        }
    }
}