using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Repositories;
namespace BancoAula.Api.Services;
public class TransferenciaService(ITransferenciaRepository repositorio) : ITransferenciaService
{
    public Task<TransferenciaResposta> Transferir(TransferenciaEntrada e)
    {
        if (e.ContaOrigemId == e.ContaDestinoId) throw new RegraException("As contas devem ser diferentes.", 422);
        if (e.Valor <= 0 || e.Valor > 9999999999.99m || decimal.Round(e.Valor, 2) != e.Valor) throw new RegraException("O valor deve ser positivo e ter no máximo duas casas decimais.", 400);
        return repositorio.Executar(e.ContaOrigemId, e.ContaDestinoId, (origem, destino) =>
        {
            if (origem is null || destino is null) throw new RegraException("Conta não encontrada.", 404);
            if (!origem.Ativa || !destino.Ativa) throw new RegraException("As contas devem estar ativas.", 422);
            if (origem.Saldo < e.Valor) throw new RegraException("Saldo insuficiente.", 422);
            if (destino.Saldo + e.Valor > 9999999999.99m) throw new RegraException("O saldo de destino excede o limite permitido.", 422);
            origem.Saldo -= e.Valor; destino.Saldo += e.Valor; var agora = DateTime.UtcNow;
            var enviada = new Transacao { ContaId = origem.Id, Descricao = $"Transferência para conta {destino.Numero}", Tipo = TipoTransacao.TransferenciaEnviada, Valor = e.Valor, DataHora = agora };
            var recebida = new Transacao { ContaId = destino.Id, Descricao = $"Transferência da conta {origem.Numero}", Tipo = TipoTransacao.TransferenciaRecebida, Valor = e.Valor, DataHora = agora };
            return (new TransferenciaResposta(origem.Id, destino.Id, e.Valor, origem.Saldo, agora), enviada, recebida);
        });
    }
    public async Task<ExtratoResposta> Extrato(int id, DateOnly? de, DateOnly? ate, TipoTransacao? tipo)
    {
        if (de.HasValue && ate.HasValue && de > ate) throw new RegraException("A data inicial deve ser anterior ou igual à final.", 400);
        if (tipo.HasValue && !Enum.IsDefined(tipo.Value)) throw new RegraException("Tipo de transação inválido.", 400);
        var dados = await repositorio.Extrato(id, de, ate, tipo) ?? throw new RegraException("Conta não encontrada.", 404);
        return new ExtratoResposta(id, dados.Saldo, dados.Transacoes.Select(t => new TransacaoResposta(t.Id, t.Descricao, t.Tipo, t.Valor, t.DataHora)).ToList());
    }
}
