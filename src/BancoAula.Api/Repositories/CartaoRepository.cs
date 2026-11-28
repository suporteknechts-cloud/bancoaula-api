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
    public Task<Cartao?> Buscar(int id) => banco.Cartoes.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
}
