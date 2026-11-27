using BancoAula.Api.Data;
using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace BancoAula.Api.Repositories;
public class MovimentacaoRepository(BancoContext banco) : IMovimentacaoRepository
{
    public async Task<T> Executar<T>(int contaId, Func<Conta?, (T Resultado, Transacao Transacao)> operacao)
    {
        await using var transacao = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var conta = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {contaId}").SingleOrDefaultAsync();
        var (resultado, registro) = operacao(conta);
        banco.Transacoes.Add(registro);
        await banco.SaveChangesAsync();
        await transacao.CommitAsync();
        return resultado;
    }
}
