using Imobly.Domain.Entities;
using Imobly.Domain.Interfaces;
using Imobly.Infrastructure.Data;
using Imobly.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Imobly.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        // Repositories
        private UsuarioRepository _usuarioRepository;
        private ImovelRepository _imovelRepository;
        private LocatarioRepository _locatarioRepository;
        private ContratoRepository _contratoRepository;
        private RecebimentoRepository _recebimentoRepository;
        private MovimentacaoFinanceiraRepository _movimentacaoFinanceiraRepository;
        private Repository<Manutencao> _manutencaoRepository;
        private Repository<Seguro> _seguroRepository;
        private Repository<HistoricoReajuste> _historicoReajusteRepository;
        private Repository<MidiaImovel> _midiaImovelRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Propriedades de acesso aos repositórios
        public IUsuarioRepository Usuarios => _usuarioRepository ??= new UsuarioRepository(_context);
        public IImovelRepository Imoveis => _imovelRepository ??= new ImovelRepository(_context);
        public ILocatarioRepository Locatarios => _locatarioRepository ??= new LocatarioRepository(_context);
        public IContratoRepository Contratos => _contratoRepository ??= new ContratoRepository(_context);
        public IRecebimentoRepository Recebimentos => _recebimentoRepository ??= new RecebimentoRepository(_context);
        public IMovimentacaoFinanceiraRepository MovimentacoesFinanceiras =>
            _movimentacaoFinanceiraRepository ??= new MovimentacaoFinanceiraRepository(_context);
        public IRepository<Manutencao> Manutencoes => _manutencaoRepository ??= new Repository<Manutencao>(_context);
        public IRepository<Seguro> Seguros => _seguroRepository ??= new Repository<Seguro>(_context);
        public IRepository<HistoricoReajuste> HistoricosReajuste =>
            _historicoReajusteRepository ??= new Repository<HistoricoReajuste>(_context);
        public IRepository<MidiaImovel> MidiasImoveis => _midiaImovelRepository ??= new Repository<MidiaImovel>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            _transaction?.Dispose();
        }
    }
}