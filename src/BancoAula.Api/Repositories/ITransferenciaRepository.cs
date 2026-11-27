using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Repositories;
public interface ITransferenciaRepository
{
    Task<T> Executar<T>(int origemId, int destinoId, Func<Conta?, Conta?, (T Resultado, Transacao Enviada, Transacao Recebida)> operacao);
    Task<(decimal Saldo, List<Transacao> Transacoes)?> Extrato(int id, DateOnly? de, DateOnly? ate, TipoTransacao? tipo);
}
