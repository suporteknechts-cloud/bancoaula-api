using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Repositories;
namespace BancoAula.Api.Services;
public class AgenciaService(IAgenciaRepository repositorio) : IAgenciaService
{
    private static AgenciaResposta Resposta(Agencia a) => new(a.Id, a.Nome, a.Cidade);
    private async Task<Agencia> Obter(int id) => await repositorio.Buscar(id) ?? throw new RegraException("Agência não encontrada.", 404);
    public async Task<List<AgenciaResposta>> Listar() => (await repositorio.Listar()).Select(Resposta).ToList();
    public async Task<AgenciaResposta> Buscar(int id) => Resposta(await Obter(id));
    public async Task<AgenciaResposta> Criar(AgenciaEntrada entrada)
    {
        var a = new Agencia { Nome = entrada.Nome.Trim(), Cidade = entrada.Cidade.Trim() };
        await repositorio.Adicionar(a); await repositorio.Salvar(); return Resposta(a);
    }
    public async Task Atualizar(int id, AgenciaEntrada entrada)
    {
        var a = await Obter(id); a.Nome = entrada.Nome.Trim(); a.Cidade = entrada.Cidade.Trim(); await repositorio.Salvar();
    }
    public async Task Remover(int id)
    {
        var a = await Obter(id);
        if (await repositorio.PossuiContas(id)) throw new RegraException("A agência possui contas vinculadas.", 409);
        repositorio.Remover(a); await repositorio.Salvar();
    }
}
