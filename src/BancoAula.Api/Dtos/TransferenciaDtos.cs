using System.ComponentModel.DataAnnotations;
using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Dtos;
public record TransferenciaEntrada([Range(1, int.MaxValue)] int ContaOrigemId, [Range(1, int.MaxValue)] int ContaDestinoId, decimal Valor);
public record TransferenciaResposta(int ContaOrigemId, int ContaDestinoId, decimal Valor, decimal SaldoOrigem, DateTime DataHora);
public record TransacaoResposta(int Id, string Descricao, TipoTransacao Tipo, decimal Valor, DateTime DataHora);
public record ExtratoResposta(int ContaId, decimal Saldo, List<TransacaoResposta> Transacoes);
