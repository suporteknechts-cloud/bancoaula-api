using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface IEmprestimoRepository
{
    Task<Emprestimo> Solicitar(int clienteId, int contaId, Func<bool, Conta?, List<Conta>, (Emprestimo Emprestimo, Transacao? Transacao)> analisar);
    Task<Emprestimo?> Buscar(int id);
    Task<List<Emprestimo>> Listar(int? clienteId, StatusEmprestimo? status);
}
