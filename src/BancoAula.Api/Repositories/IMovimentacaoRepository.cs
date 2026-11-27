using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface IMovimentacaoRepository
{
    Task<T> Executar<T>(int contaId, Func<Conta?, (T Resultado, Transacao Transacao)> operacao);
}
