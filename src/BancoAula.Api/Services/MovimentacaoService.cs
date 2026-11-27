using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Repositories;
namespace BancoAula.Api.Services;
public class MovimentacaoService(IMovimentacaoRepository repositorio) : IMovimentacaoService
{
    public Task<MovimentacaoResposta> Depositar(int contaId, decimal valor) => Movimentar(contaId, valor, TipoTransacao.Deposito);
    public Task<MovimentacaoResposta> Sacar(int contaId, decimal valor) => Movimentar(contaId, valor, TipoTransacao.Saque);
    private Task<MovimentacaoResposta> Movimentar(int id, decimal valor, TipoTransacao tipo)
    {
        if (valor <= 0 || valor > 9999999999.99m || decimal.Round(valor, 2) != valor) throw new RegraException("O valor deve ser positivo e ter no máximo duas casas decimais.", 400);
        return repositorio.Executar(id, conta =>
        {
            if (conta is null) throw new RegraException("Conta não encontrada.", 404);
            if (!conta.Ativa) throw new RegraException("Conta inativa.", 422);
            if (tipo == TipoTransacao.Saque && conta.Saldo < valor) throw new RegraException("Saldo insuficiente.", 422);
            if (tipo == TipoTransacao.Deposito && conta.Saldo + valor > 9999999999.99m) throw new RegraException("O saldo excede o limite permitido.", 422);
            conta.Saldo += tipo == TipoTransacao.Deposito ? valor : -valor;
            var agora = DateTime.UtcNow;
            var registro = new Transacao { ContaId = id, Tipo = tipo, Descricao = tipo.ToString(), Valor = valor, DataHora = agora };
            return (new MovimentacaoResposta(id, conta.Saldo, tipo.ToString(), valor, agora), registro);
        });
    }
}
