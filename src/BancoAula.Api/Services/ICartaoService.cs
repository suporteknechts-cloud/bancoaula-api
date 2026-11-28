using BancoAula.Api.Dtos;
namespace BancoAula.Api.Services;
public interface ICartaoService
{
    Task<CartaoResposta> Emitir(CartaoEntrada entrada);
    Task<CartaoResposta> Buscar(int id);
    Task<CompraResposta> Comprar(int id, CompraEntrada entrada);
    Task<FaturaResposta> Fatura(int id, int mes, int ano);
    Task<PagamentoFaturaResposta> PagarFatura(int id, PagamentoFaturaEntrada entrada);
}

