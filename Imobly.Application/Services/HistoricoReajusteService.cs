using AutoMapper;
using Imobly.Application.DTOs.Reajustes;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class HistoricoReajusteService : IHistoricoReajusteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HistoricoReajusteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<HistoricoReajusteDto> GetByIdAsync(Guid id, Guid usuarioId)
        {
            var historico = await _unitOfWork.HistoricosReajuste.GetByIdAsync(id);
            if (historico == null)
            {
                throw new KeyNotFoundException("Histórico de reajuste não encontrado");
            }

            // Carregar contrato para validação
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(historico.ContratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            return _mapper.Map<HistoricoReajusteDto>(historico);
        }

        public async Task<IEnumerable<HistoricoReajusteDto>> GetByContratoAsync(Guid contratoId, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(contratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            var historicos = await _unitOfWork.HistoricosReajuste.FindAsync(h => h.ContratoId == contratoId);
            return _mapper.Map<IEnumerable<HistoricoReajusteDto>>(historicos.OrderByDescending(h => h.DataReajuste));
        }

        public async Task<HistoricoReajusteDto> CreateAsync(CriarHistoricoReajusteDto dto, Guid usuarioId)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(dto.ContratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            // Validar se o novo valor é diferente do atual
            if (dto.ValorNovo == contrato.ValorAluguel)
            {
                throw new ArgumentException("O novo valor deve ser diferente do valor atual");
            }

            // Criar histórico
            var historico = new HistoricoReajuste
            {
                ContratoId = dto.ContratoId,
                ValorAnterior = contrato.ValorAluguel,
                ValorNovo = dto.ValorNovo,
                DataReajuste = DateTime.UtcNow,
                IndiceUtilizado = dto.IndiceUtilizado
            };

            // Atualizar valor no contrato
            contrato.ValorAluguel = dto.ValorNovo;
            contrato.Atualizar();

            await _unitOfWork.HistoricosReajuste.AddAsync(historico);
            _unitOfWork.Contratos.Update(contrato);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<HistoricoReajusteDto>(historico);
        }

        public async Task<HistoricoReajusteDto> UpdateAsync(Guid id, CriarHistoricoReajusteDto dto, Guid usuarioId)
        {
            var historico = await _unitOfWork.HistoricosReajuste.GetByIdAsync(id);
            if (historico == null)
            {
                throw new KeyNotFoundException("Histórico de reajuste não encontrado");
            }

            // Carregar contrato para validação
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(historico.ContratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            // Atualizar histórico
            historico.ValorNovo = dto.ValorNovo;
            historico.IndiceUtilizado = dto.IndiceUtilizado;
            historico.DataReajuste = DateTime.UtcNow;
            historico.Atualizar();

            // Reverter valor anterior no contrato se necessário
            contrato.ValorAluguel = dto.ValorNovo;
            contrato.Atualizar();

            _unitOfWork.HistoricosReajuste.Update(historico);
            _unitOfWork.Contratos.Update(contrato);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<HistoricoReajusteDto>(historico);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid usuarioId)
        {
            var historico = await _unitOfWork.HistoricosReajuste.GetByIdAsync(id);
            if (historico == null)
            {
                throw new KeyNotFoundException("Histórico de reajuste não encontrado");
            }

            // Carregar contrato para validação
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(historico.ContratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Acesso negado");
            }

            // Não permitir exclusão se for o único histórico ou se o contrato estiver usando esse valor
            var historicosContrato = await _unitOfWork.HistoricosReajuste.FindAsync(h => h.ContratoId == historico.ContratoId);
            if (historicosContrato.Count() == 1)
            {
                throw new InvalidOperationException("Não é possível excluir o único histórico de reajuste do contrato");
            }

            // Se for o último reajuste, reverter para o valor anterior
            var ultimoHistorico = historicosContrato.OrderByDescending(h => h.DataReajuste).First();
            if (ultimoHistorico.Id == historico.Id)
            {
                var penultimoHistorico = historicosContrato
                    .Where(h => h.Id != historico.Id)
                    .OrderByDescending(h => h.DataReajuste)
                    .FirstOrDefault();

                if (penultimoHistorico != null)
                {
                    contrato.ValorAluguel = penultimoHistorico.ValorNovo;
                    contrato.Atualizar();
                    _unitOfWork.Contratos.Update(contrato);
                }
            }

            _unitOfWork.HistoricosReajuste.Remove(historico);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<decimal> CalcularReajusteAsync(decimal valorAtual, string indice, decimal percentual)
        {
            // Aqui você pode integrar com APIs de índices reais (INCC, IPCA, etc.)
            // Por enquanto, usaremos um cálculo simples baseado no percentual
            return valorAtual * (1 + percentual / 100);
        }

        public async Task<IEnumerable<HistoricoReajusteDto>> GetUltimosReajustesAsync(Guid usuarioId, int quantidade = 10)
        {
            var imoveis = await _unitOfWork.Imoveis.GetByUsuarioIdAsync(usuarioId);
            var imoveisIds = imoveis.Select(i => i.Id);

            var contratos = await _unitOfWork.Contratos.FindAsync(c =>
                imoveisIds.Contains(c.ImovelId));
            var contratosIds = contratos.Select(c => c.Id);

            var historicos = await _unitOfWork.HistoricosReajuste.FindAsync(h =>
                contratosIds.Contains(h.ContratoId));

            return _mapper.Map<IEnumerable<HistoricoReajusteDto>>(
                historicos.OrderByDescending(h => h.DataReajuste).Take(quantidade));
        }

        public async Task<object> SugerirReajusteAsync(Guid contratoId, Guid usuarioId, string indice)
        {
            var contrato = await _unitOfWork.Contratos.GetWithDetailsAsync(contratoId);
            if (contrato == null || contrato.Imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Contrato não encontrado ou acesso negado");
            }

            // Valores padrão de percentual por índice (exemplo)
            var percentuais = new Dictionary<string, decimal>
            {
                { "INCC", 5.2m },
                { "IPCA", 4.5m },
                { "IGP-M", 6.1m },
                { "IPC-FIPE", 3.8m }
            };

            if (!percentuais.TryGetValue(indice.ToUpper(), out var percentual))
            {
                percentual = 4.0m; // Percentual padrão
            }

            var novoValor = await CalcularReajusteAsync(contrato.ValorAluguel, indice, percentual);
            var aumentoAbsoluto = novoValor - contrato.ValorAluguel;
            var percentualAumento = (aumentoAbsoluto / contrato.ValorAluguel) * 100;

            return new
            {
                ValorAtual = contrato.ValorAluguel,
                NovoValor = novoValor,
                PercentualSugerido = percentual,
                PercentualAumento = percentualAumento,
                AumentoAbsoluto = aumentoAbsoluto,
                Indice = indice,
                DataSugestao = DateTime.UtcNow,
                Observacao = $"Reajuste sugerido baseado no índice {indice}"
            };
        }
    }
}