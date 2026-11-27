using BancoAula.Api.Dtos;
using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Services;
public interface IEmprestimoService
{
    Task<EmprestimoResposta> Solicitar(EmprestimoEntrada entrada);
    Task<EmprestimoResposta> Buscar(int id);
    Task<List<EmprestimoResposta>> Listar(int? clienteId, StatusEmprestimo? status);
}
