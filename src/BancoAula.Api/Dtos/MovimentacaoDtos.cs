using System.ComponentModel.DataAnnotations;
namespace BancoAula.Api.Dtos;
public record MovimentacaoEntrada(decimal Valor);
public record MovimentacaoResposta(int ContaId, decimal Saldo, string Tipo, decimal Valor, DateTime DataHora);

