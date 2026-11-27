using System.Data;
using BancoAula.Api.Data;
using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Repositories;
public class TransferenciaRepository(BancoContext banco) : ITransferenciaRepository
{
    public async Task<T> Executar<T>(int origemId, int destinoId, Func<Conta?, Conta?, (T Resultado, Transacao Enviada, Transacao Recebida)> operacao)
    {
        await using var tx = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var primeiroId = Math.Min(origemId, destinoId); var segundoId = Math.Max(origemId, destinoId);
        var primeiro = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {primeiroId}").SingleOrDefaultAsync();
        var segundo = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {segundoId}").SingleOrDefaultAsync();
        var (resultado, enviada, recebida) = operacao(origemId == primeiroId ? primeiro : segundo, destinoId == primeiroId ? primeiro : segundo);
        banco.Transacoes.AddRange(enviada, recebida); await banco.SaveChangesAsync(); await tx.CommitAsync(); return resultado;
    }
    public async Task<(decimal Saldo, List<Transacao> Transacoes)?> Extrato(int id, DateOnly? de, DateOnly? ate, TipoTransacao? tipo)
    {
        await using var tx = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var conta = await banco.Contas.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        if (conta is null) return null;
        var q = banco.Transacoes.AsNoTracking().Where(x => x.ContaId == id);
        if (de.HasValue) { var inicio = de.Value.ToDateTime(TimeOnly.MinValue); q = q.Where(x => x.DataHora >= inicio); }
        if (ate.HasValue && ate.Value < DateOnly.MaxValue) { var fim = ate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue); q = q.Where(x => x.DataHora < fim); }
        if (tipo.HasValue) q = q.Where(x => x.Tipo == tipo);
        var itens = await q.OrderByDescending(x => x.DataHora).ThenByDescending(x => x.Id).ToListAsync();
        await tx.CommitAsync(); return (conta.Saldo, itens);
    }
}
