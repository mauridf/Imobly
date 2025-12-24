using AutoMapper;
using Imobly.Application.DTOs.Recebimentos;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class RecebimentoService : IRecebimentoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecebimentoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecebimentoDto>> GetByContratoAsync(Guid contratoId, Guid usuarioId)
        {
            // Validar se o contrato pertence ao usuário
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(contratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            var recebimentos = await _unitOfWork.Recebimentos.GetByContratoIdAsync(contratoId);
            return _mapper.Map<IEnumerable<RecebimentoDto>>(recebimentos);
        }

        public async Task<RecebimentoDto> RegistrarPagamentoAsync(Guid id, RegistrarPagamentoDto dto, Guid usuarioId)
        {
            var recebimento = await _unitOfWork.Recebimentos.GetByIdAsync(id);
            if (recebimento == null)
            {
                throw new KeyNotFoundException("Recebimento não encontrado");
            }

            // Carregar contrato para validação
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(recebimento.ContratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            // Registrar pagamento
            recebimento.RegistrarPagamento(dto.ValorPago, dto.DataPagamento);
            recebimento.Atualizar();

            _unitOfWork.Recebimentos.Update(recebimento);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<RecebimentoDto>(recebimento);
        }

        public async Task<IEnumerable<RecebimentoDto>> GerarRecebimentosAsync(GerarRecebimentosDto dto, Guid usuarioId)
        {
            // Validar se o contrato pertence ao usuário
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(dto.ContratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            // Limpar recebimentos existentes (se houver)
            var recebimentosExistentes = await _unitOfWork.Recebimentos.GetByContratoIdAsync(dto.ContratoId);
            if (recebimentosExistentes.Any())
            {
                _unitOfWork.Recebimentos.RemoveRange(recebimentosExistentes);
            }

            // Gerar recebimentos mensais
            var recebimentos = new List<Recebimento>();
            var dataAtual = dto.DataInicio;

            while (dataAtual <= dto.DataFim)
            {
                var recebimento = new Recebimento
                {
                    ContratoId = dto.ContratoId,
                    Competencia = new DateTime(dataAtual.Year, dataAtual.Month, 1),
                    ValorPrevisto = dto.ValorAluguel,
                    Status = StatusRecebimento.Aguardando
                };

                recebimentos.Add(recebimento);
                dataAtual = dataAtual.AddMonths(1);
            }

            await _unitOfWork.Recebimentos.AddRangeAsync(recebimentos);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<RecebimentoDto>>(recebimentos);
        }

        public async Task<IEnumerable<RecebimentoDto>> GetPendentesByUsuarioAsync(Guid usuarioId)
        {
            var recebimentos = await _unitOfWork.Recebimentos.GetPendentesByUsuarioIdAsync(usuarioId);
            return _mapper.Map<IEnumerable<RecebimentoDto>>(recebimentos);
        }

        public async Task<IEnumerable<RecebimentoDto>> GetAtrasadosByUsuarioAsync(Guid usuarioId)
        {
            var recebimentos = await _unitOfWork.Recebimentos.GetAtrasadosByUsuarioIdAsync(usuarioId);
            return _mapper.Map<IEnumerable<RecebimentoDto>>(recebimentos);
        }

        public async Task<decimal> GetTotalRecebidoNoMesAsync(Guid usuarioId, int mes, int ano)
        {
            return await _unitOfWork.Recebimentos.GetTotalRecebidoNoMesAsync(usuarioId, mes, ano);
        }

        // Método auxiliar para verificar recebimentos vencidos
        public async Task VerificarRecebimentosVencidos()
        {
            var hoje = DateTime.UtcNow;
            var recebimentosAguardando = await _unitOfWork.Recebimentos.FindAsync(r =>
                r.Status == StatusRecebimento.Aguardando);

            foreach (var recebimento in recebimentosAguardando)
            {
                if (recebimento.EstaVencido())
                {
                    recebimento.Status = StatusRecebimento.Atrasado;
                    recebimento.Atualizar();
                    _unitOfWork.Recebimentos.Update(recebimento);
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}