using BancoAula.Api.Dtos;
using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Services;
public interface IContaService
{
    Task<List<ContaResposta>> Listar(int? clienteId, int? agenciaId, TipoConta? tipo, bool? ativa);
    Task<ContaResposta> Buscar(int id);
    Task<ContaResposta> Criar(ContaEntrada entrada);
    Task Encerrar(int id);
}
