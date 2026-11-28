using System.Data;
using BancoAula.Api.Data;
using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Repositories;
public class CartaoRepository(BancoContext banco) : ICartaoRepository
{
    public async Task<Cartao> Emitir(int contaId, Func<Conta?, bool, Cartao> criar)
    {
        await using var tx = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var conta = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {contaId}").SingleOrDefaultAsync();
        var existe = conta is not null && await banco.Cartoes.AnyAsync(c => c.ContaId == contaId && c.Status == StatusCartao.Ativo);
        var cartao = criar(conta, existe);
        banco.Cartoes.Add(cartao); await banco.SaveChangesAsync(); await tx.CommitAsync(); return cartao;
    }
    public async Task<T> Comprar<T>(int id, Func<Cartao?, (T Resultado, CompraCartao Compra)> operacao)
    {
        await using var tx = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var cartao = await banco.Cartoes.FromSqlInterpolated($"SELECT * FROM Cartoes WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}").SingleOrDefaultAsync();
        var (resultado, compra) = operacao(cartao);
        banco.ComprasCartao.Add(compra); await banco.SaveChangesAsync(); await tx.CommitAsync(); return resultado;
    }
    public Task<Cartao?> BuscarComCompras(int id) => banco.Cartoes.AsNoTracking().Include(c => c.Compras).SingleOrDefaultAsync(c => c.Id == id);
    public async Task<T> PagarFatura<T>(int id, Func<Cartao?, Conta?, (T Resultado, Transacao Transacao)> operacao)
    {
        var contaId = await banco.Cartoes.AsNoTracking().Where(c => c.Id == id).Select(c => (int?)c.ContaId).SingleOrDefaultAsync();
        await using var tx = await banco.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        Conta? conta = null;
        if (contaId.HasValue) conta = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {contaId.Value}").SingleOrDefaultAsync();
        var cartao = await banco.Cartoes.FromSqlInterpolated($"SELECT * FROM Cartoes WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}").Include(c => c.Compras).SingleOrDefaultAsync();
        var (resultado, transacao) = operacao(cartao, conta);
        banco.Transacoes.Add(transacao); await banco.SaveChangesAsync(); await tx.CommitAsync(); return resultado;
    }
    public Task<Cartao?> Buscar(int id) => banco.Cartoes.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
}

