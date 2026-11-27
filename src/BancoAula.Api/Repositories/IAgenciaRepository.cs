using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface IAgenciaRepository
{
    Task<List<Agencia>> Listar();
    Task<Agencia?> Buscar(int id);
    Task<bool> PossuiContas(int id);
    Task Adicionar(Agencia agencia);
    Task Salvar();
    void Remover(Agencia agencia);
}
