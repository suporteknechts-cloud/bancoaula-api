namespace BancoAula.Api.Exceptions;
public class RegraException(string mensagem, int status) : Exception(mensagem) { public int Status { get; } = status; }
