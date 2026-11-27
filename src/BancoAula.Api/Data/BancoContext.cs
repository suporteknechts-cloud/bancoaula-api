using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Data;
public class BancoContext(DbContextOptions<BancoContext> options) : DbContext(options)
{
    public DbSet<Agencia> Agencias => Set<Agencia>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();
    public DbSet<Parcela> Parcelas => Set<Parcela>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Emprestimo>().Property(x => x.ValorSolicitado).HasPrecision(12, 2);
        modelBuilder.Entity<Emprestimo>().Property(x => x.TaxaJurosMensal).HasPrecision(8, 6);
        modelBuilder.Entity<Emprestimo>().Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Emprestimo>().HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Emprestimo>().HasOne(x => x.Conta).WithMany().HasForeignKey(x => x.ContaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Parcela>().Property(x => x.Valor).HasPrecision(12, 2);
        modelBuilder.Entity<Parcela>().Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Parcela>().HasIndex(x => new { x.EmprestimoId, x.Numero }).IsUnique();
        modelBuilder.Entity<Parcela>().HasOne(x => x.Emprestimo).WithMany(x => x.Parcelas).HasForeignKey(x => x.EmprestimoId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Cliente>().HasIndex(x => x.Cpf).IsUnique();
        modelBuilder.Entity<Cliente>().Property(x => x.Cpf).HasMaxLength(11);
        modelBuilder.Entity<Conta>().HasIndex(x => x.Numero).IsUnique();
        modelBuilder.Entity<Conta>().Property(x => x.Numero).HasMaxLength(50);
        modelBuilder.Entity<Conta>().Property(x => x.Saldo).HasPrecision(12, 2);
        modelBuilder.Entity<Transacao>().Property(x => x.Valor).HasPrecision(12, 2);
        modelBuilder.Entity<Conta>().Property(x => x.Tipo).HasConversion<string>().HasMaxLength(30);
        modelBuilder.Entity<Transacao>().Property(x => x.Tipo).HasConversion<string>().HasMaxLength(40);
        modelBuilder.Entity<Conta>().HasOne(x => x.Cliente).WithMany(x => x.Contas).HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Conta>().HasOne(x => x.Agencia).WithMany(x => x.Contas).HasForeignKey(x => x.AgenciaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Transacao>().HasOne(x => x.Conta).WithMany(x => x.Transacoes).HasForeignKey(x => x.ContaId).OnDelete(DeleteBehavior.Restrict);
    }
}

