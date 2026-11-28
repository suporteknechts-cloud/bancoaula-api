namespace BancoAula.Api.Models.Entities;
public enum StatusEmprestimo { Aprovado, Recusado, Quitado }
public enum StatusParcela { Pendente, Paga }
public class Emprestimo
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public int ContaId { get; set; }
    public Conta Conta { get; set; } = null!;
    public decimal ValorSolicitado { get; set; }
    public decimal TaxaJurosMensal { get; set; }
    public int QuantidadeParcelas { get; set; }
    public StatusEmprestimo Status { get; set; }
    public string? MotivoRecusa { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
}
public class Parcela
{
    public int Id { get; set; }
    public int EmprestimoId { get; set; }
    public Emprestimo Emprestimo { get; set; } = null!;
    public int Numero { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public StatusParcela Status { get; set; }
    public DateTime? DataPagamento { get; set; }
}
