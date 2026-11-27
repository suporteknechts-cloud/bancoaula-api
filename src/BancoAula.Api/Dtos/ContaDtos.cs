using System.ComponentModel.DataAnnotations;
using BancoAula.Api.Models.Entities;
namespace BancoAula.Api.Dtos;
public record ContaEntrada([Range(1, int.MaxValue)] int ClienteId, [Range(1, int.MaxValue)] int AgenciaId, [Required, StringLength(50)] string Numero, [Required, EnumDataType(typeof(TipoConta))] TipoConta? Tipo);
public record ContaResposta(int Id, int ClienteId, string ClienteNome, int AgenciaId, string AgenciaNome, string Numero, TipoConta Tipo, decimal Saldo, bool Ativa);
