using System.Data;
using BancoAula.Api.Data;
using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Repositories;
public class EmprestimoRepository(BancoContext banco) : IEmprestimoRepository
{
    public async Task<Emprestimo> Solicitar(int clienteId, int contaId, Func<bool, Conta?, List<Conta>, (Emprestimo Emprestimo, Transacao? Transacao)> analisar)
    {
        await using var tx = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var clienteExiste = await banco.Clientes.AnyAsync(c => c.Id == clienteId);
        var contas = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE ClienteId = {clienteId} OR Id = {contaId}").OrderBy(c => c.Id).ToListAsync();
        var conta = contas.SingleOrDefault(c => c.Id == contaId);
        var (emprestimo, transacao) = analisar(clienteExiste, conta, contas.Where(c => c.ClienteId == clienteId && c.Ativa).ToList());
        banco.Emprestimos.Add(emprestimo);
        if (transacao is not null) banco.Transacoes.Add(transacao);
        await banco.SaveChangesAsync(); await tx.CommitAsync(); return emprestimo;
    }
    public async Task<T> Pagar<T>(int emprestimoId, Func<Emprestimo?, Conta?, (T Resultado, Transacao Transacao)> operacao)
    {
        var contaId = await banco.Emprestimos.AsNoTracking().Where(e => e.Id == emprestimoId).Select(e => (int?)e.ContaId).SingleOrDefaultAsync();
        await using var tx = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        Conta? conta = null;
        if (contaId.HasValue) conta = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {contaId.Value}").SingleOrDefaultAsync();
        var emprestimo = await banco.Emprestimos.FromSqlInterpolated($"SELECT * FROM Emprestimos WITH (UPDLOCK, ROWLOCK) WHERE Id = {emprestimoId}").Include(e => e.Parcelas).SingleOrDefaultAsync();
        var (resultado, transacao) = operacao(emprestimo, conta);
        banco.Transacoes.Add(transacao); await banco.SaveChangesAsync(); await tx.CommitAsync(); return resultado;
    }
    public Task<Emprestimo?> Buscar(int id) => banco.Emprestimos.AsNoTracking().Include(e => e.Parcelas).SingleOrDefaultAsync(e => e.Id == id);
    public Task<List<Emprestimo>> Listar(int? clienteId, StatusEmprestimo? status)
    {
        var q = banco.Emprestimos.AsNoTracking().Include(e => e.Parcelas).AsQueryable();
        if (clienteId.HasValue) q = q.Where(e => e.ClienteId == clienteId);
        if (status.HasValue) q = q.Where(e => e.Status == status);
        return q.OrderByDescending(e => e.DataSolicitacao).ThenByDescending(e => e.Id).ToListAsync();
    }
}


