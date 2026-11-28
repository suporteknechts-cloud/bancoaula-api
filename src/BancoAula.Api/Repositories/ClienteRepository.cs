using BancoAula.Api.Data;
using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Repositories;
public class ClienteRepository(BancoContext banco) : IClienteRepository
{
    public Task<List<Cliente>> Listar() => banco.Clientes.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    public Task<Cliente?> Buscar(int id) => banco.Clientes.FirstOrDefaultAsync(x => x.Id == id);
    public Task<bool> CpfExiste(string cpf, int? excetoId = null) => banco.Clientes.AnyAsync(x => x.Cpf == cpf && x.Id != excetoId);
    public Task<bool> PossuiEmprestimos(int id) => banco.Emprestimos.AnyAsync(x => x.ClienteId == id);
    public Task<bool> PossuiContas(int id) => banco.Contas.AnyAsync(x => x.ClienteId == id);
    public async Task Adicionar(Cliente cliente) => await banco.Clientes.AddAsync(cliente);
    public Task Salvar() => banco.SaveChangesAsync();
    public void Remover(Cliente cliente) => banco.Clientes.Remove(cliente);
}

