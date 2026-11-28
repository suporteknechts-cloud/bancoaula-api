using BancoAula.Api.Dtos;
namespace BancoAula.Api.Services;
public interface ICartaoService
{
    Task<CartaoResposta> Emitir(CartaoEntrada entrada);
    Task<CartaoResposta> Buscar(int id);
}
