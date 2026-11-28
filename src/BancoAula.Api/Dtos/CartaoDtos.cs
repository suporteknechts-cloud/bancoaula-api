using System.ComponentModel.DataAnnotations;
using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Dtos;
public record CartaoEntrada([Range(1, int.MaxValue)] int ContaId, [Required, StringLength(100)] string NomeImpresso, decimal Limite);
public record CartaoResposta(int Id, int ContaId, string NumeroMascarado, string NomeImpresso, decimal Limite, decimal LimiteDisponivel, StatusCartao Status, DateTime DataEmissao);
