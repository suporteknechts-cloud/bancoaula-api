using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace BancoAula.Api.Services;
public class ClienteService(IClienteRepository repositorio) : IClienteService
{
    private static ClienteResposta Resposta(Cliente c) => new(c.Id, c.Nome, c.Cpf, c.Email, c.Cidade);
    private async Task<Cliente> Obter(int id) => await repositorio.Buscar(id) ?? throw new RegraException("Cliente não encontrado.", 404);
    private async Task ValidarCpf(string cpf, int? id = null) { if (await repositorio.CpfExiste(cpf, id)) throw new RegraException("CPF já cadastrado.", 409); }
    private async Task Salvar()
    {
        try { await repositorio.Salvar(); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627)) { throw new RegraException("CPF já cadastrado.", 409); }
    }
    public async Task<List<ClienteResposta>> Listar() => (await repositorio.Listar()).Select(Resposta).ToList();
    public async Task<ClienteResposta> Buscar(int id) => Resposta(await Obter(id));
    public async Task<ClienteResposta> Criar(ClienteEntrada entrada)
    {
        await ValidarCpf(entrada.Cpf);
        var cliente = new Cliente { Nome = entrada.Nome.Trim(), Cpf = entrada.Cpf, Email = entrada.Email.Trim(), Cidade = entrada.Cidade.Trim() };
        await repositorio.Adicionar(cliente); await Salvar(); return Resposta(cliente);
    }
    public async Task Atualizar(int id, ClienteEntrada entrada)
    {
        var cliente = await Obter(id); await ValidarCpf(entrada.Cpf, id);
        cliente.Nome = entrada.Nome.Trim(); cliente.Cpf = entrada.Cpf; cliente.Email = entrada.Email.Trim(); cliente.Cidade = entrada.Cidade.Trim(); await Salvar();
    }
    public async Task Remover(int id)
    {
        var cliente = await Obter(id);
        if (await repositorio.PossuiContas(id)) throw new RegraException("O cliente possui contas vinculadas.", 409);
        repositorio.Remover(cliente); await repositorio.Salvar();
    }
}
