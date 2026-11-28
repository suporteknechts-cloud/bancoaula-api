using System.ComponentModel.DataAnnotations;
namespace BancoAula.Api.Dtos;
public record CompraEntrada([Required, StringLength(200)] string Descricao, decimal Valor);
public record CompraResposta(int Id, string Descricao, decimal Valor, DateTime DataCompra, bool Paga);
public record FaturaResposta(int CartaoId, int Mes, int Ano, decimal Total, decimal TotalPago, decimal EmAberto, List<CompraResposta> Compras);
public record PagamentoFaturaEntrada([Range(1, 12)] int Mes, [Range(1, 9999)] int Ano);
public record PagamentoFaturaResposta(int CartaoId, int Mes, int Ano, decimal ValorPago, decimal SaldoConta, decimal LimiteDisponivel);
