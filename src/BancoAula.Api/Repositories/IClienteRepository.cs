using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface IClienteRepository
{
    Task<List<Cliente>> Listar();
    Task<Cliente?> Buscar(int id);
    Task<bool> CpfExiste(string cpf, int? excetoId = null);
    Task<bool> PossuiContas(int id);
    Task<bool> PossuiEmprestimos(int id);
    Task Adicionar(Cliente cliente);
    Task Salvar();
    void Remover(Cliente cliente);
}

