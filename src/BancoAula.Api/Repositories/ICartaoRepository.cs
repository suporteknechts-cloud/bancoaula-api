using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface ICartaoRepository
{
    Task<Cartao> Emitir(int contaId, Func<Conta?, bool, Cartao> criar);
    Task<Cartao?> Buscar(int id);
}
