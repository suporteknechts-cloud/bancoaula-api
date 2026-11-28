using BancoAula.Api.Data;
using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Repositories;
public class ContaRepository(BancoContext banco) : IContaRepository
{
    private IQueryable<Conta> ComDados() => banco.Contas.Include(x => x.Cliente).Include(x => x.Agencia);
    public Task<List<Conta>> Listar(int? clienteId, int? agenciaId, TipoConta? tipo, bool? ativa)
    {
        var q = ComDados().AsNoTracking();
        if (clienteId.HasValue) q = q.Where(x => x.ClienteId == clienteId);
        if (agenciaId.HasValue) q = q.Where(x => x.AgenciaId == agenciaId);
        if (tipo.HasValue) q = q.Where(x => x.Tipo == tipo);
        if (ativa.HasValue) q = q.Where(x => x.Ativa == ativa);
        return q.OrderBy(x => x.Id).ToListAsync();
    }
    public Task<Conta?> Buscar(int id) => ComDados().FirstOrDefaultAsync(x => x.Id == id);
    public async Task EncerrarComBloqueio(int id, Action<Conta?, bool, bool> validar)
    {
        await using var tx = await banco.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var conta = await banco.Contas.FromSqlInterpolated($"SELECT * FROM Contas WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}").SingleOrDefaultAsync();
        var aberto = conta is not null && await banco.Emprestimos.AnyAsync(x => x.ContaId == id && x.Status == StatusEmprestimo.Aprovado);
        var cartaoAtivo = conta is not null && await banco.Cartoes.AnyAsync(c => c.ContaId == id && c.Status == StatusCartao.Ativo);
        validar(conta, aberto, cartaoAtivo);
        await banco.SaveChangesAsync(); await tx.CommitAsync();
    }
    public Task<bool> NumeroExiste(string numero) => banco.Contas.AnyAsync(x => x.Numero == numero);
    public Task<bool> ClienteExiste(int id) => banco.Clientes.AnyAsync(x => x.Id == id);
    public Task<bool> AgenciaExiste(int id) => banco.Agencias.AnyAsync(x => x.Id == id);
    public async Task Adicionar(Conta conta) => await banco.Contas.AddAsync(conta);
    public Task Salvar() => banco.SaveChangesAsync();
}



