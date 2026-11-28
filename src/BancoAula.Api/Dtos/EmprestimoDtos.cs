using System.ComponentModel.DataAnnotations;
using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Dtos;
public record EmprestimoEntrada([Range(1, int.MaxValue)] int ClienteId, [Range(1, int.MaxValue)] int ContaId, decimal ValorSolicitado, [Range(6, 48)] int QuantidadeParcelas);
public record ParcelaResposta(int Numero, decimal Valor, DateTime DataVencimento, StatusParcela Status, DateTime? DataPagamento);
public record EmprestimoResposta(int Id, int ClienteId, int ContaId, decimal ValorSolicitado, decimal TaxaJurosMensal, int QuantidadeParcelas, StatusEmprestimo Status, string? MotivoRecusa, DateTime DataSolicitacao, decimal ValorTotalAPagar, List<ParcelaResposta> Parcelas);
public record PagamentoParcelaResposta(int EmprestimoId, int Numero, decimal Valor, decimal SaldoConta, StatusEmprestimo StatusEmprestimo, DateTime DataPagamento);
