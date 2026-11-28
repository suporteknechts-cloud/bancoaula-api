using System.Security.Cryptography;
using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Repositories;
namespace BancoAula.Api.Services;
public class CartaoService(ICartaoRepository repositorio) : ICartaoService
{
    private static CartaoResposta Resposta(Cartao c) => new(c.Id, c.ContaId, "**** **** **** " + c.Numero[^4..], c.NomeImpresso, c.Limite, c.LimiteDisponivel, c.Status, c.DataEmissao);
    public async Task<CartaoResposta> Buscar(int id) => Resposta(await repositorio.Buscar(id) ?? throw new RegraException("Cartão não encontrado.", 404));
    public Task AlterarStatus(int id, bool bloquear) => repositorio.AlterarStatus(id, (cartao, conta, outroAtivo) =>
    {
        if (cartao is null) throw new RegraException("Cartão não encontrado.", 404);
        if (!bloquear && (conta is null || !conta.Ativa || conta.Tipo != TipoConta.Corrente)) throw new RegraException("É necessária uma conta corrente ativa para desbloquear.", 422);
        if (!bloquear && outroAtivo) throw new RegraException("A conta já possui outro cartão ativo.", 422);
        cartao.Status = bloquear ? StatusCartao.Bloqueado : StatusCartao.Ativo;
    });
    private static void ValidarPeriodo(int mes, int ano)
    {
        if (mes < 1 || mes > 12 || ano < 1 || ano > 9999) throw new RegraException("Mês ou ano inválido.", 400);
    }
    public async Task<CompraResposta> Comprar(int id, CompraEntrada entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada.Descricao)) throw new RegraException("Descrição é obrigatória.", 400);
        if (entrada.Valor <= 0 || entrada.Valor > 9999999999.99m || decimal.Round(entrada.Valor, 2) != entrada.Valor) throw new RegraException("Valor inválido.", 400);
        var compra = await repositorio.Comprar(id, cartao =>
        {
            if (cartao is null) throw new RegraException("Cartão não encontrado.", 404);
            if (cartao.Status != StatusCartao.Ativo) throw new RegraException("Cartão bloqueado.", 422);
            if (entrada.Valor > cartao.LimiteDisponivel) throw new RegraException("Limite insuficiente.", 422);
            cartao.LimiteDisponivel -= entrada.Valor;
            var c = new CompraCartao { CartaoId = id, Descricao = entrada.Descricao.Trim(), Valor = entrada.Valor, DataCompra = DateTime.UtcNow, Paga = false };
            return (c, c);
        });
        return new CompraResposta(compra.Id, compra.Descricao, compra.Valor, compra.DataCompra, compra.Paga);
    }
    public async Task<FaturaResposta> Fatura(int id, int mes, int ano)
    {
        ValidarPeriodo(mes, ano);
        var cartao = await repositorio.BuscarComCompras(id) ?? throw new RegraException("Cartão não encontrado.", 404);
        var compras = cartao.Compras.Where(c => c.DataCompra.Month == mes && c.DataCompra.Year == ano).OrderBy(c => c.DataCompra).ThenBy(c => c.Id).ToList();
        var total = compras.Sum(c => c.Valor); var pago = compras.Where(c => c.Paga).Sum(c => c.Valor);
        return new FaturaResposta(id, mes, ano, total, pago, total - pago, compras.Select(c => new CompraResposta(c.Id, c.Descricao, c.Valor, c.DataCompra, c.Paga)).ToList());
    }
    public Task<PagamentoFaturaResposta> PagarFatura(int id, PagamentoFaturaEntrada entrada)
    {
        ValidarPeriodo(entrada.Mes, entrada.Ano);
        return repositorio.PagarFatura(id, (cartao, conta) =>
        {
            if (cartao is null) throw new RegraException("Cartão não encontrado.", 404);
            if (conta is null) throw new RegraException("Conta não encontrada.", 404);
            if (!conta.Ativa) throw new RegraException("Conta inativa.", 422);
            var compras = cartao.Compras.Where(c => c.DataCompra.Month == entrada.Mes && c.DataCompra.Year == entrada.Ano && !c.Paga).ToList();
            var total = compras.Sum(c => c.Valor);
            if (total <= 0) throw new RegraException("Fatura sem valor em aberto.", 422);
            if (conta.Saldo < total) throw new RegraException("Saldo insuficiente.", 422);
            if (cartao.LimiteDisponivel + total > cartao.Limite) throw new RegraException("Inconsistência no limite do cartão.", 422);
            conta.Saldo -= total; cartao.LimiteDisponivel += total; foreach (var compra in compras) compra.Paga = true;
            var agora = DateTime.UtcNow;
            var transacao = new Transacao { ContaId = conta.Id, Descricao = $"Pagamento de fatura {entrada.Mes:D2}/{entrada.Ano}", Tipo = TipoTransacao.PagamentoFatura, Valor = total, DataHora = agora };
            return (new PagamentoFaturaResposta(id, entrada.Mes, entrada.Ano, total, conta.Saldo, cartao.LimiteDisponivel), transacao);
        });
    }
    public async Task<CartaoResposta> Emitir(CartaoEntrada e)
    {
        if (e.Limite < 500 || e.Limite > 20000 || decimal.Round(e.Limite, 2) != e.Limite) throw new RegraException("Limite deve estar entre R$ 500 e R$ 20.000, com até duas casas decimais.", 400);
        if (string.IsNullOrWhiteSpace(e.NomeImpresso)) throw new RegraException("Nome impresso é obrigatório.", 400);
        var cartao = await repositorio.Emitir(e.ContaId, (conta, existe) =>
        {
            if (conta is null) throw new RegraException("Conta não encontrada.", 404);
            if (!conta.Ativa || conta.Tipo != TipoConta.Corrente) throw new RegraException("O cartão exige conta corrente ativa.", 422);
            if (existe) throw new RegraException("A conta já possui cartão ativo.", 422);
            var digitos = new char[16]; for (var i = 0; i < digitos.Length; i++) digitos[i] = (char)('0' + RandomNumberGenerator.GetInt32(10));
            return new Cartao { ContaId = conta.Id, Numero = new string(digitos), NomeImpresso = e.NomeImpresso.Trim(), Limite = e.Limite, LimiteDisponivel = e.Limite, Status = StatusCartao.Ativo, DataEmissao = DateTime.UtcNow };
        });
        return Resposta(cartao);
    }
}
