using BancoAula.Api.Data;
using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Repositories;
public class AgenciaRepository(BancoContext banco) : IAgenciaRepository
{
    public Task<List<Agencia>> Listar() => banco.Agencias.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    public Task<Agencia?> Buscar(int id) => banco.Agencias.FirstOrDefaultAsync(x => x.Id == id);
    public Task<bool> PossuiContas(int id) => banco.Contas.AnyAsync(x => x.AgenciaId == id);
    public async Task Adicionar(Agencia agencia) => await banco.Agencias.AddAsync(agencia);
    public Task Salvar() => banco.SaveChangesAsync();
    public void Remover(Agencia agencia) => banco.Agencias.Remove(agencia);
}
