using BancoAula.Api.Dtos;
namespace BancoAula.Api.Services;
public interface IClienteService
{
    Task<List<ClienteResposta>> Listar();
    Task<ClienteResposta> Buscar(int id);
    Task<ClienteResposta> Criar(ClienteEntrada entrada);
    Task Atualizar(int id, ClienteEntrada entrada);
    Task Remover(int id);
}
