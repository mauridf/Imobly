using Imobly.Application.DTOs.Dashboard;
using Imobly.Application.Interfaces;
using Imobly.Domain.Enums;
using Imobly.Domain.Interfaces;

namespace Imobly.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardResumoDto> GetResumoAsync(Guid usuarioId)
        {
            var imoveis = await _unitOfWork.Imoveis.GetByUsuarioIdAsync(usuarioId);
            var contratos = await _unitOfWork.Contratos.GetAtivosByUsuarioIdAsync(usuarioId);
            var locatarios = await _unitOfWork.Locatarios.GetAllAsync();
            var recebimentosPendentes = await _unitOfWork.Recebimentos.GetPendentesByUsuarioIdAsync(usuarioId);

            // Calcular totais
            var totalImoveis = imoveis.Count();
            var imoveisAtivos = imoveis.Count(i => i.Ativo);
            var totalLocatarios = locatarios.Count();
            var contratosAtivos = contratos.Count();
            var recebimentosPendentesCount = recebimentosPendentes.Count();

            // Calcular manutenções pendentes
            var manutencoes = await _unitOfWork.Manutencoes.FindAsync(m =>
                m.Imovel.UsuarioId == usuarioId && m.Status == StatusManutencao.Pendente);
            var manutencoesPendentes = manutencoes.Count();

            // Calcular receita e despesa do mês atual
            var hoje = DateTime.UtcNow;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);

            var receitaMensal = await _unitOfWork.Recebimentos.GetTotalRecebidoNoMesAsync(
                usuarioId, hoje.Month, hoje.Year);

            var movimentacoes = await _unitOfWork.MovimentacoesFinanceiras.GetByPeriodoAsync(
                usuarioId, inicioMes, fimMes);

            var despesaMensal = movimentacoes
                .Where(m => m.Tipo == TipoMovimentacao.Despesa && m.Status == StatusMovimentacao.Pago)
                .Sum(m => m.Valor);

            return new DashboardResumoDto
            {
                TotalImoveis = totalImoveis,
                ImoveisAtivos = imoveisAtivos,
                TotalLocatarios = totalLocatarios,
                ContratosAtivos = contratosAtivos,
                RecebimentosPendentes = recebimentosPendentesCount,
                ManutencoesPendentes = manutencoesPendentes,
                ReceitaMensal = receitaMensal,
                DespesaMensal = despesaMensal,
                SaldoMensal = receitaMensal - despesaMensal
            };
        }

        public async Task<IEnumerable<GraficoReceitaDespesaDto>> GetGraficoReceitaDespesaAsync(
            Guid usuarioId, int meses = 6)
        {
            var resultado = new List<GraficoReceitaDespesaDto>();
            var hoje = DateTime.UtcNow;

            for (int i = meses - 1; i >= 0; i--)
            {
                var data = hoje.AddMonths(-i);
                var inicioMes = new DateTime(data.Year, data.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                // Calcular receita do mês
                var receita = await _unitOfWork.Recebimentos.GetTotalRecebidoNoMesAsync(
                    usuarioId, data.Month, data.Year);

                // Calcular despesa do mês
                var movimentacoes = await _unitOfWork.MovimentacoesFinanceiras.GetByPeriodoAsync(
                    usuarioId, inicioMes, fimMes);

                var despesa = movimentacoes
                    .Where(m => m.Tipo == TipoMovimentacao.Despesa && m.Status == StatusMovimentacao.Pago)
                    .Sum(m => m.Valor);

                resultado.Add(new GraficoReceitaDespesaDto
                {
                    Mes = data.ToString("MMM/yy", new System.Globalization.CultureInfo("pt-BR")),
                    Receita = receita,
                    Despesa = despesa
                });
            }

            return resultado;
        }

        public async Task<IEnumerable<ContratoProximoVencimentoDto>> GetContratosProximosVencimentoAsync(
            Guid usuarioId, int dias = 30)
        {
            var contratos = await _unitOfWork.Contratos.GetAtivosByUsuarioIdAsync(usuarioId);
            var hoje = DateTime.UtcNow;
            var dataLimite = hoje.AddDays(dias);

            var contratosProximos = contratos
                .Where(c => c.DataFim <= dataLimite && c.DataFim >= hoje)
                .Select(c => new ContratoProximoVencimentoDto
                {
                    Id = c.Id,
                    ImovelTitulo = c.Imovel.Titulo,
                    LocatarioNome = c.Locatario.Nome,
                    DataFim = c.DataFim,
                    DiasParaVencimento = (c.DataFim - hoje).Days
                })
                .OrderBy(c => c.DataFim)
                .ToList();

            return contratosProximos;
        }

        public async Task<IEnumerable<ManutencaoPendenteDto>> GetManutencoesPendentesAsync(Guid usuarioId)
        {
            var manutencoes = await _unitOfWork.Manutencoes.FindAsync(m =>
                m.Imovel.UsuarioId == usuarioId &&
                m.Status == StatusManutencao.Pendente);

            var hoje = DateTime.UtcNow;

            var manutencoesPendentes = manutencoes
                .Select(m => new ManutencaoPendenteDto
                {
                    Id = m.Id,
                    ImovelTitulo = m.Imovel.Titulo,
                    Descricao = m.Descricao.Length > 50 ? m.Descricao.Substring(0, 50) + "..." : m.Descricao,
                    Data = m.Data,
                    Valor = m.Valor
                })
                .OrderBy(m => m.Data)
                .ToList();

            return manutencoesPendentes;
        }

        // Método adicional para estatísticas detalhadas
        public async Task<object> GetEstatisticasDetalhadasAsync(Guid usuarioId)
        {
            var hoje = DateTime.UtcNow;
            var inicioAno = new DateTime(hoje.Year, 1, 1);
            var fimAno = new DateTime(hoje.Year, 12, 31);

            // Total recebido no ano
            var totalRecebidoAno = 0m;
            for (int mes = 1; mes <= 12; mes++)
            {
                totalRecebidoAno += await _unitOfWork.Recebimentos.GetTotalRecebidoNoMesAsync(usuarioId, mes, hoje.Year);
            }

            // Total despesas no ano
            var movimentacoesAno = await _unitOfWork.MovimentacoesFinanceiras.GetByPeriodoAsync(
                usuarioId, inicioAno, fimAno);

            var totalDespesasAno = movimentacoesAno
                .Where(m => m.Tipo == TipoMovimentacao.Despesa && m.Status == StatusMovimentacao.Pago)
                .Sum(m => m.Valor);

            // Locatários inadimplentes
            var contratos = await _unitOfWork.Contratos.GetAtivosByUsuarioIdAsync(usuarioId);
            var locatariosIds = contratos.Select(c => c.LocatarioId).Distinct();
            var locatarios = await _unitOfWork.Locatarios.FindAsync(l => locatariosIds.Contains(l.Id));
            var locatariosInadimplentes = locatarios.Count(l => l.Status == StatusLocatario.Inadimplente);

            // Recebimentos atrasados
            var recebimentosAtrasados = await _unitOfWork.Recebimentos.GetAtrasadosByUsuarioIdAsync(usuarioId);
            var totalAtrasado = recebimentosAtrasados.Sum(r => r.ValorPrevisto);

            return new
            {
                TotalRecebidoAno = totalRecebidoAno,
                TotalDespesasAno = totalDespesasAno,
                SaldoAno = totalRecebidoAno - totalDespesasAno,
                LocatariosInadimplentes = locatariosInadimplentes,
                TotalAtrasado = totalAtrasado,
                PercentualInadimplencia = contratos.Any() ?
                    (decimal)locatariosInadimplentes / contratos.Count() * 100 : 0,
                MesMaisRentavel = await GetMesMaisRentavelAsync(usuarioId, hoje.Year)
            };
        }

        private async Task<string> GetMesMaisRentavelAsync(Guid usuarioId, int ano)
        {
            var receitasPorMes = new Dictionary<int, decimal>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var receita = await _unitOfWork.Recebimentos.GetTotalRecebidoNoMesAsync(usuarioId, mes, ano);
                receitasPorMes[mes] = receita;
            }

            if (receitasPorMes.All(r => r.Value == 0))
                return "Nenhum";

            var mesMaisRentavel = receitasPorMes.OrderByDescending(r => r.Value).First();
            var cultura = new System.Globalization.CultureInfo("pt-BR");
            return cultura.DateTimeFormat.GetMonthName(mesMaisRentavel.Key);
        }

        public async Task<EstatisticasDetalhadasDto> GetEstatisticasDetalhadasAsync(Guid usuarioId)
        {
            var hoje = DateTime.UtcNow;
            var inicioAno = new DateTime(hoje.Year, 1, 1);
            var fimAno = new DateTime(hoje.Year, 12, 31);

            // Total recebido no ano
            var totalRecebidoAno = 0m;
            for (int mes = 1; mes <= 12; mes++)
            {
                totalRecebidoAno += await _unitOfWork.Recebimentos.GetTotalRecebidoNoMesAsync(usuarioId, mes, hoje.Year);
            }

            // Total despesas no ano
            var movimentacoesAno = await _unitOfWork.MovimentacoesFinanceiras.GetByPeriodoAsync(
                usuarioId, inicioAno, fimAno);

            var totalDespesasAno = movimentacoesAno
                .Where(m => m.Tipo == TipoMovimentacao.Despesa && m.Status == StatusMovimentacao.Pago)
                .Sum(m => m.Valor);

            // Locatários inadimplentes
            var contratos = await _unitOfWork.Contratos.GetAtivosByUsuarioIdAsync(usuarioId);
            var locatariosIds = contratos.Select(c => c.LocatarioId).Distinct();
            var locatarios = await _unitOfWork.Locatarios.FindAsync(l => locatariosIds.Contains(l.Id));
            var locatariosInadimplentes = locatarios.Count(l => l.Status == StatusLocatario.Inadimplente);

            // Recebimentos atrasados
            var recebimentosAtrasados = await _unitOfWork.Recebimentos.GetAtrasadosByUsuarioIdAsync(usuarioId);
            var totalAtrasado = recebimentosAtrasados.Sum(r => r.ValorPrevisto);

            // Contratos vencendo no próximo mês
            var proximoMes = hoje.AddMonths(1);
            var contratosVencendoProximoMes = contratos
                .Count(c => c.DataFim.Month == proximoMes.Month && c.DataFim.Year == proximoMes.Year);

            // Previsão de receita do próximo mês
            var previsaoReceitaProximoMes = contratos
                .Where(c => c.DataFim >= proximoMes || c.DataFim >= hoje)
                .Sum(c => c.ValorAluguel);

            return new EstatisticasDetalhadasDto
            {
                TotalRecebidoAno = totalRecebidoAno,
                TotalDespesasAno = totalDespesasAno,
                SaldoAno = totalRecebidoAno - totalDespesasAno,
                LocatariosInadimplentes = locatariosInadimplentes,
                TotalAtrasado = totalAtrasado,
                PercentualInadimplencia = contratos.Any() ?
                    (decimal)locatariosInadimplentes / contratos.Count() * 100 : 0,
                MesMaisRentavel = await GetMesMaisRentavelAsync(usuarioId, hoje.Year),
                ContratosVencendoProximoMes = contratosVencendoProximoMes,
                PrevisaoReceitaProximoMes = previsaoReceitaProximoMes
            };
        }
    }
}