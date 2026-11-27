using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Services;
public class ContaService(IContaRepository repositorio) : IContaService
{
    private static ContaResposta Resposta(Conta c) => new(c.Id, c.ClienteId, c.Cliente.Nome, c.AgenciaId, c.Agencia.Nome, c.Numero, c.Tipo, c.Saldo, c.Ativa);
    private async Task<Conta> Obter(int id) => await repositorio.Buscar(id) ?? throw new RegraException("Conta não encontrada.", 404);
    public async Task<List<ContaResposta>> Listar(int? clienteId, int? agenciaId, TipoConta? tipo, bool? ativa)
    {
        if (tipo.HasValue && !Enum.IsDefined(tipo.Value)) throw new RegraException("Tipo de conta inválido.", 400);
        return (await repositorio.Listar(clienteId, agenciaId, tipo, ativa)).Select(Resposta).ToList();
    }
    public async Task<ContaResposta> Buscar(int id) => Resposta(await Obter(id));
    public async Task<ContaResposta> Criar(ContaEntrada entrada)
    {
        var numero = entrada.Numero.Trim();
        if (!await repositorio.ClienteExiste(entrada.ClienteId)) throw new RegraException("Cliente não encontrado.", 404);
        if (!await repositorio.AgenciaExiste(entrada.AgenciaId)) throw new RegraException("Agência não encontrada.", 404);
        if (await repositorio.NumeroExiste(numero)) throw new RegraException("Número de conta já cadastrado.", 409);
        var conta = new Conta { ClienteId = entrada.ClienteId, AgenciaId = entrada.AgenciaId, Numero = numero, Tipo = entrada.Tipo!.Value, Saldo = 0, Ativa = true };
        await repositorio.Adicionar(conta);
        try { await repositorio.Salvar(); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627)) { throw new RegraException("Número de conta já cadastrado.", 409); }
        return Resposta(await Obter(conta.Id));
    }
    public async Task Encerrar(int id)
    {
        var conta = await Obter(id);
        if (conta.Saldo != 0) throw new RegraException("A conta deve ter saldo zero para ser encerrada.", 422);
        if (await repositorio.PossuiEmprestimoAberto(id)) throw new RegraException("A conta possui empréstimo em aberto.", 422);
        conta.Ativa = false; await repositorio.Salvar();
    }
}

