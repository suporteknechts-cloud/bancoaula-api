using System.ComponentModel.DataAnnotations;
namespace BancoAula.Api.Dtos;
public record AgenciaEntrada([Required, StringLength(150, MinimumLength = 1)] string Nome, [Required, StringLength(100, MinimumLength = 1)] string Cidade);
public record AgenciaResposta(int Id, string Nome, string Cidade);
