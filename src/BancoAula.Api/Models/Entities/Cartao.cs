namespace BancoAula.Api.Models.Entities;
public enum StatusCartao { Ativo, Bloqueado }
public class Cartao
{
    public int Id { get; set; }
    public int ContaId { get; set; }
    public Conta Conta { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string NomeImpresso { get; set; } = null!;
    public decimal Limite { get; set; }
    public decimal LimiteDisponivel { get; set; }
    public StatusCartao Status { get; set; }
    public DateTime DataEmissao { get; set; }
    public ICollection<CompraCartao> Compras { get; set; } = new List<CompraCartao>();
}
public class CompraCartao
{
    public int Id { get; set; }
    public int CartaoId { get; set; }
    public Cartao Cartao { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime DataCompra { get; set; }
    public bool Paga { get; set; }
}
