using BancoAula.Api.Dtos;
using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Services;
public interface ITransferenciaService
{
    Task<TransferenciaResposta> Transferir(TransferenciaEntrada entrada);
    Task<ExtratoResposta> Extrato(int contaId, DateOnly? de, DateOnly? ate, TipoTransacao? tipo);
}
