using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface IContaRepository
{
    Task<List<Conta>> Listar(int? clienteId, int? agenciaId, TipoConta? tipo, bool? ativa);
    Task<Conta?> Buscar(int id);
    Task<bool> NumeroExiste(string numero);
    Task<bool> ClienteExiste(int id);
    Task<bool> AgenciaExiste(int id);
    Task Adicionar(Conta conta);
    Task Salvar();
}
