using AutoMapper;
using Imobly.Application.DTOs.Contratos;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class ContratoService : IContratoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContratoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ContratoDto> GetByIdAsync(Guid id, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(id);

            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            return _mapper.Map<ContratoDto>(contrato);
        }

        public async Task<IEnumerable<ContratoDto>> GetByUsuarioAsync(Guid usuarioId)
        {
            var contratos = await _unitOfWork.Contratos.GetAtivosByUsuarioIdAsync(usuarioId);
            return _mapper.Map<IEnumerable<ContratoDto>>(contratos);
        }

        public async Task<ContratoDto> CreateAsync(CriarContratoDto dto, Guid usuarioId)
        {
            // Validar se imóvel pertence ao usuário
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(dto.ImovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            // Validar se imóvel já tem contrato ativo
            var temContratoAtivo = await _unitOfWork.Contratos.HasContratoAtivoAsync(dto.ImovelId);
            if (temContratoAtivo)
            {
                throw new InvalidOperationException("Imóvel já possui um contrato ativo");
            }

            // Validar locatário
            var locatario = await _unitOfWork.Locatarios.GetByIdAsync(dto.LocatarioId);
            if (locatario == null)
            {
                throw new KeyNotFoundException("Locatário não encontrado");
            }

            var contrato = _mapper.Map<Contrato>(dto);
            await _unitOfWork.Contratos.AddAsync(contrato);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContratoDto>(contrato);
        }

        public async Task<ContratoDto> UpdateAsync(Guid id, AtualizarContratoDto dto, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(id);

            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            if (dto.Status != null)
            {
                contrato.Status = Enum.Parse<StatusContrato>(dto.Status);
            }

            if (dto.DataFim.HasValue) contrato.DataFim = dto.DataFim.Value;
            if (dto.ValorAluguel.HasValue) contrato.ValorAluguel = dto.ValorAluguel.Value;
            if (dto.ValorSeguro.HasValue) contrato.ValorSeguro = dto.ValorSeguro.Value;
            if (dto.DiaVencimento.HasValue) contrato.DiaVencimento = dto.DiaVencimento.Value;

            contrato.Atualizar();
            _unitOfWork.Contratos.Update(contrato);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContratoDto>(contrato);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(id);

            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            if (contrato.Status == StatusContrato.Ativo)
            {
                throw new InvalidOperationException("Não é possível excluir contrato ativo");
            }

            _unitOfWork.Contratos.Remove(contrato);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> EncerrarAsync(Guid id, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(id);

            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            contrato.Encerrar();
            _unitOfWork.Contratos.Update(contrato);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> SuspenderAsync(Guid id, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(id);

            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            contrato.Suspender();
            _unitOfWork.Contratos.Update(contrato);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ReativarAsync(Guid id, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(id);

            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            contrato.Reativar();
            _unitOfWork.Contratos.Update(contrato);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<ContratoComDetalhesDto> GetWithDetailsAsync(Guid id, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(id);

            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            var dto = _mapper.Map<ContratoComDetalhesDto>(contrato);

            // Calcular métricas
            dto.TotalRecebimentos = contrato.Recebimentos.Count;
            dto.RecebimentosPagos = contrato.Recebimentos.Count(r => r.Status == StatusRecebimento.Pago || r.Status == StatusRecebimento.Adiantado);
            dto.RecebimentosAtrasados = contrato.Recebimentos.Count(r => r.Status == StatusRecebimento.Atrasado);
            dto.TotalRecebido = contrato.Recebimentos
                .Where(r => r.Status == StatusRecebimento.Pago || r.Status == StatusRecebimento.Adiantado)
                .Sum(r => r.ValorPago);

            return dto;
        }
    }
}