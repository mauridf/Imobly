using AutoMapper;
using Imobly.Application.DTOs.Locatarios;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class LocatarioService : ILocatarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocatarioService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LocatarioDto> GetByIdAsync(Guid id)
        {
            var locatario = await _unitOfWork.Locatarios.GetByIdAsync(id);
            if (locatario == null)
            {
                throw new KeyNotFoundException("Locatário não encontrado");
            }

            return _mapper.Map<LocatarioDto>(locatario);
        }

        public async Task<IEnumerable<LocatarioDto>> GetAllAsync()
        {
            var locatarios = await _unitOfWork.Locatarios.GetAllAsync();
            return _mapper.Map<IEnumerable<LocatarioDto>>(locatarios);
        }

        public async Task<LocatarioDto> CreateAsync(CriarLocatarioDto dto)
        {
            // Validar CPF único
            if (await _unitOfWork.Locatarios.CpfExistsAsync(dto.CPF))
            {
                throw new ArgumentException("CPF já cadastrado");
            }

            var locatario = _mapper.Map<Locatario>(dto);
            await _unitOfWork.Locatarios.AddAsync(locatario);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<LocatarioDto>(locatario);
        }

        public async Task<LocatarioDto> UpdateAsync(Guid id, AtualizarLocatarioDto dto)
        {
            var locatario = await _unitOfWork.Locatarios.GetByIdAsync(id);
            if (locatario == null)
            {
                throw new KeyNotFoundException("Locatário não encontrado");
            }

            _mapper.Map(dto, locatario);
            locatario.Atualizar();

            _unitOfWork.Locatarios.Update(locatario);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<LocatarioDto>(locatario);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var locatario = await _unitOfWork.Locatarios.GetByIdAsync(id);
            if (locatario == null)
            {
                throw new KeyNotFoundException("Locatário não encontrado");
            }

            // Verificar se há contratos ativos
            var contratos = await _unitOfWork.Contratos.GetByLocatarioIdAsync(id);
            if (contratos.Any(c => c.Status == Domain.Enums.StatusContrato.Ativo))
            {
                throw new InvalidOperationException("Não é possível excluir locatário com contratos ativos");
            }

            _unitOfWork.Locatarios.Remove(locatario);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<LocatarioDto>> SearchAsync(string searchTerm)
        {
            var locatarios = await _unitOfWork.Locatarios.SearchAsync(searchTerm);
            return _mapper.Map<IEnumerable<LocatarioDto>>(locatarios);
        }

        public async Task<bool> MarcarComoAdimplenteAsync(Guid id)
        {
            var locatario = await _unitOfWork.Locatarios.GetByIdAsync(id);
            if (locatario == null)
            {
                throw new KeyNotFoundException("Locatário não encontrado");
            }

            locatario.MarcarComoAdimplente();
            _unitOfWork.Locatarios.Update(locatario);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> MarcarComoInadimplenteAsync(Guid id)
        {
            var locatario = await _unitOfWork.Locatarios.GetByIdAsync(id);
            if (locatario == null)
            {
                throw new KeyNotFoundException("Locatário não encontrado");
            }

            locatario.MarcarComoInadimplente();
            _unitOfWork.Locatarios.Update(locatario);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}