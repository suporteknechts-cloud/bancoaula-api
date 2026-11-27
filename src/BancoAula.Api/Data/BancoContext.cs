using BancoAula.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Data;
public class BancoContext(DbContextOptions<BancoContext> options) : DbContext(options)
{
    public DbSet<Agencia> Agencias => Set<Agencia>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
