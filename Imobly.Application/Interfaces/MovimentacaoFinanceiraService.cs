using AutoMapper;
using Imobly.Application.DTOs.Movimentacoes;
using Imobly.Application.Interfaces;
using Imobly.Domain.Entities;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class MovimentacaoFinanceiraService : IMovimentacaoFinanceiraService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MovimentacaoFinanceiraService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MovimentacaoFinanceiraDto> GetByIdAsync(Guid id, Guid usuarioId)
        {
            var movimentacao = await _unitOfWork.MovimentacoesFinanceiras.GetByIdAsync(id);
            if (movimentacao == null)
            {
                throw new KeyNotFoundException("Movimentação financeira não encontrada");
            }

            // Se tiver imóvel associado, validar acesso
            if (movimentacao.ImovelId.HasValue)
            {
                var imovel = await _unitOfWork.Imoveis.GetByIdAsync(movimentacao.ImovelId.Value);
                if (imovel == null || imovel.UsuarioId != usuarioId)
                {
                    throw new KeyNotFoundException("Acesso negado");
                }
            }

            return _mapper.Map<MovimentacaoFinanceiraDto>(movimentacao);
        }

        public async Task<IEnumerable<MovimentacaoFinanceiraDto>> GetByImovelAsync(Guid imovelId, Guid usuarioId)
        {
            var imovel = await _unitOfWork.Imoveis.GetByIdAsync(imovelId);
            if (imovel == null || imovel.UsuarioId != usuarioId)
            {
                throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
            }

            var movimentacoes = await _unitOfWork.MovimentacoesFinanceiras.GetByImovelIdAsync(imovelId);
            return _mapper.Map<IEnumerable<MovimentacaoFinanceiraDto>>(movimentacoes);
        }

        public async Task<IEnumerable<MovimentacaoFinanceiraDto>> GetByPeriodoAsync(
            Guid usuarioId, DateTime inicio, DateTime fim)
        {
            var movimentacoes = await _unitOfWork.MovimentacoesFinanceiras.GetByPeriodoAsync(usuarioId, inicio, fim);
            return _mapper.Map<IEnumerable<MovimentacaoFinanceiraDto>>(movimentacoes);
        }

        public async Task<IEnumerable<MovimentacaoFinanceiraDto>> GetByCategoriaAsync(string categoria, Guid usuarioId)
        {
            if (!Enum.TryParse<CategoriaMovimentacao>(categoria, true, out var categoriaEnum))
            {
                throw new ArgumentException("Categoria inválida");
            }

            var imoveis = await _unitOfWork.Imoveis.GetByUsuarioIdAsync(usuarioId);
            var imoveisIds = imoveis.Select(i => i.Id);

            var movimentacoes = await _unitOfWork.MovimentacoesFinanceiras.FindAsync(m =>
                imoveisIds.Contains(m.ImovelId.Value) && m.Categoria == categoriaEnum);

            return _mapper.Map<IEnumerable<MovimentacaoFinanceiraDto>>(movimentacoes);
        }

        public async Task<MovimentacaoFinanceiraDto> CreateAsync(CriarMovimentacaoFinanceiraDto dto, Guid usuarioId)
        {
            // Validar imóvel se fornecido
            if (dto.ImovelId.HasValue)
            {
                var imovel = await _unitOfWork.Imoveis.GetByIdAsync(dto.ImovelId.Value);
                if (imovel == null || imovel.UsuarioId != usuarioId)
                {
                    throw new KeyNotFoundException("Imóvel não encontrado ou acesso negado");
                }
            }

            var movimentacao = _mapper.Map<MovimentacaoFinanceira>(dto);
            await _unitOfWork.MovimentacoesFinanceiras.AddAsync(movimentacao);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MovimentacaoFinanceiraDto>(movimentacao);
        }

        public async Task<MovimentacaoFinanceiraDto> UpdateAsync(
            Guid id, AtualizarMovimentacaoFinanceiraDto dto, Guid usuarioId)
        {
            var movimentacao = await _unitOfWork.MovimentacoesFinanceiras.GetByIdAsync(id);
            if (movimentacao == null)
            {
                throw new KeyNotFoundException("Movimentação financeira não encontrada");
            }

            // Validar acesso
            if (movimentacao.ImovelId.HasValue)
            {
                var imovel = await _unitOfWork.Imoveis.GetByIdAsync(movimentacao.ImovelId.Value);
                if (imovel == null || imovel.UsuarioId != usuarioId)
                {
                    throw new KeyNotFoundException("Acesso negado");
                }
            }

            _mapper.Map(dto, movimentacao);

            // Se status for alterado para Pago, atualizar data
            if (dto.Status == "Pago" && movimentacao.Status != StatusMovimentacao.Pago)
            {
                movimentacao.Status = StatusMovimentacao.Pago;
            }
            else if (dto.Status == "Pendente" && movimentacao.Status != StatusMovimentacao.Pendente)
            {
                movimentacao.Status = StatusMovimentacao.Pendente;
            }

            movimentacao.Atualizar();
            _unitOfWork.MovimentacoesFinanceiras.Update(movimentacao);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MovimentacaoFinanceiraDto>(movimentacao);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid usuarioId)
        {
            var movimentacao = await _unitOfWork.MovimentacoesFinanceiras.GetByIdAsync(id);
            if (movimentacao == null)
            {
                throw new KeyNotFoundException("Movimentação financeira não encontrada");
            }

            // Validar acesso
            if (movimentacao.ImovelId.HasValue)
            {
                var imovel = await _unitOfWork.Imoveis.GetByIdAsync(movimentacao.ImovelId.Value);
                if (imovel == null || imovel.UsuarioId != usuarioId)
                {
                    throw new KeyNotFoundException("Acesso negado");
                }
            }

            _unitOfWork.MovimentacoesFinanceiras.Remove(movimentacao);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> RegistrarPagamentoAsync(Guid id, Guid usuarioId)
        {
            var movimentacao = await _unitOfWork.MovimentacoesFinanceiras.GetByIdAsync(id);
            if (movimentacao == null)
            {
                throw new KeyNotFoundException("Movimentação financeira não encontrada");
            }

            // Validar acesso
            if (movimentacao.ImovelId.HasValue)
            {
                var imovel = await _unitOfWork.Imoveis.GetByIdAsync(movimentacao.ImovelId.Value);
                if (imovel == null || imovel.UsuarioId != usuarioId)
                {
                    throw new KeyNotFoundException("Acesso negado");
                }
            }

            movimentacao.Status = StatusMovimentacao.Pago;
            movimentacao.Atualizar();

            _unitOfWork.MovimentacoesFinanceiras.Update(movimentacao);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<decimal> GetSaldoPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim)
        {
            return await _unitOfWork.MovimentacoesFinanceiras.GetSaldoPeriodoAsync(usuarioId, inicio, fim);
        }

        // Método auxiliar para gerar relatório financeiro
        public async Task<object> GerarRelatorioFinanceiroAsync(Guid usuarioId, int ano)
        {
            var resultado = new List<object>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var inicioMes = new DateTime(ano, mes, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                var movimentacoes = await _unitOfWork.MovimentacoesFinanceiras.GetByPeriodoAsync(
                    usuarioId, inicioMes, fimMes);

                var receitas = movimentacoes
                    .Where(m => m.Tipo == TipoMovimentacao.Receita && m.Status == StatusMovimentacao.Pago)
                    .Sum(m => m.Valor);

                var despesas = movimentacoes
                    .Where(m => m.Tipo == TipoMovimentacao.Despesa && m.Status == StatusMovimentacao.Pago)
                    .Sum(m => m.Valor);

                var saldo = receitas - despesas;

                resultado.Add(new
                {
                    Mes = inicioMes.ToString("MMM/yy", new System.Globalization.CultureInfo("pt-BR")),
                    Receitas = receitas,
                    Despesas = despesas,
                    Saldo = saldo,
                    DespesasPorCategoria = movimentacoes
                        .Where(m => m.Tipo == TipoMovimentacao.Despesa)
                        .GroupBy(m => m.Categoria)
                        .Select(g => new
                        {
                            Categoria = g.Key.ToString(),
                            Total = g.Sum(m => m.Valor)
                        })
                        .ToList()
                });
            }

            return resultado;
        }
    }
}