using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface ICartaoRepository
{
    Task<Cartao> Emitir(int contaId, Func<Conta?, bool, Cartao> criar);
    Task<Cartao?> Buscar(int id);
    Task<T> Comprar<T>(int id, Func<Cartao?, (T Resultado, CompraCartao Compra)> operacao);
    Task<Cartao?> BuscarComCompras(int id);
    Task<T> PagarFatura<T>(int id, Func<Cartao?, Conta?, (T Resultado, Transacao Transacao)> operacao);
}

