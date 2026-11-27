using System.ComponentModel.DataAnnotations;
namespace BancoAula.Api.Dtos;
public record ClienteEntrada([Required, StringLength(150)] string Nome, [Required, RegularExpression("^[0-9]{11}$", ErrorMessage = "CPF deve conter 11 dígitos.")] string Cpf, [Required, EmailAddress, StringLength(254)] string Email, [Required, StringLength(100)] string Cidade);
public record ClienteResposta(int Id, string Nome, string Cpf, string Email, string Cidade);
