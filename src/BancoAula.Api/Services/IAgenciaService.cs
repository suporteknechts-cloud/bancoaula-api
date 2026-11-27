using BancoAula.Api.Dtos;
namespace BancoAula.Api.Services;
public interface IAgenciaService
{
    Task<List<AgenciaResposta>> Listar();
    Task<AgenciaResposta> Buscar(int id);
    Task<AgenciaResposta> Criar(AgenciaEntrada entrada);
    Task Atualizar(int id, AgenciaEntrada entrada);
    Task Remover(int id);
}
