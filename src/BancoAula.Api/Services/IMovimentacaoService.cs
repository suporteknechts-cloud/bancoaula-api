using BancoAula.Api.Dtos;
namespace BancoAula.Api.Services;
public interface IMovimentacaoService
{
    Task<MovimentacaoResposta> Depositar(int contaId, decimal valor);
    Task<MovimentacaoResposta> Sacar(int contaId, decimal valor);
}
