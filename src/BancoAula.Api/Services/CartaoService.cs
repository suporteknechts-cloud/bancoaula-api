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
