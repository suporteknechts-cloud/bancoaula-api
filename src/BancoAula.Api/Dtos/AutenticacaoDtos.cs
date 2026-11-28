using System.ComponentModel.DataAnnotations;
namespace BancoAula.Api.Dtos;
public record CredenciaisEntrada([Required, EmailAddress] string Email, [Required, StringLength(128, MinimumLength = 8)] string Senha);
public record TokenResposta(string Token, DateTime ExpiraEmUtc);
