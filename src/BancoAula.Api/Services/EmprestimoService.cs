using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Repositories;
namespace BancoAula.Api.Services;
public class EmprestimoService(IEmprestimoRepository repositorio) : IEmprestimoService
{
    private const decimal Taxa = 0.02m;
    private static EmprestimoResposta Resposta(Emprestimo e) => new(e.Id, e.ClienteId, e.ContaId, e.ValorSolicitado, e.TaxaJurosMensal, e.QuantidadeParcelas, e.Status, e.MotivoRecusa, e.DataSolicitacao, e.Parcelas.Sum(p => p.Valor), e.Parcelas.OrderBy(p => p.Numero).Select(p => new ParcelaResposta(p.Numero, p.Valor, p.DataVencimento, p.Status, p.DataPagamento)).ToList());
    public async Task<EmprestimoResposta> Solicitar(EmprestimoEntrada e)
    {
        if (e.ValorSolicitado < 500 || e.ValorSolicitado > 50000 || decimal.Round(e.ValorSolicitado, 2) != e.ValorSolicitado) throw new RegraException("Valor solicitado deve estar entre R$ 500 e R$ 50.000, com até duas casas decimais.", 400);
        if (e.QuantidadeParcelas < 6 || e.QuantidadeParcelas > 48) throw new RegraException("A quantidade de parcelas deve estar entre 6 e 48.", 400);
        var resultado = await repositorio.Solicitar(e.ClienteId, e.ContaId, (existe, conta, contas) =>
        {
            if (!existe || conta is null) throw new RegraException("Cliente ou conta não encontrado.", 404);
            if (conta.ClienteId != e.ClienteId || !conta.Ativa) throw new RegraException("A conta deve pertencer ao cliente e estar ativa.", 422);
            var agora = DateTime.UtcNow;
            var emprestimo = new Emprestimo { ClienteId = e.ClienteId, ContaId = e.ContaId, ValorSolicitado = e.ValorSolicitado, TaxaJurosMensal = Taxa, QuantidadeParcelas = e.QuantidadeParcelas, DataSolicitacao = agora };
            if (contas.Sum(c => c.Saldo) < e.ValorSolicitado * 0.1m)
            {
                emprestimo.Status = StatusEmprestimo.Recusado; emprestimo.MotivoRecusa = "Saldo total das contas ativas inferior a 10% do valor solicitado."; return (emprestimo, null);
            }
            if (conta.Saldo + e.ValorSolicitado > 9999999999.99m) throw new RegraException("O crédito excede o saldo máximo permitido.", 422);
            emprestimo.Status = StatusEmprestimo.Aprovado;
            decimal fator = 1m; for (int i = 0; i < e.QuantidadeParcelas; i++) fator *= 1m + Taxa;
            decimal valorParcela = decimal.Round(e.ValorSolicitado * Taxa * fator / (fator - 1m), 2, MidpointRounding.AwayFromZero);
            for (int numero = 1; numero <= e.QuantidadeParcelas; numero++) emprestimo.Parcelas.Add(new Parcela { Numero = numero, Valor = valorParcela, DataVencimento = agora.AddMonths(numero), Status = StatusParcela.Pendente });
            conta.Saldo += e.ValorSolicitado;
            var transacao = new Transacao { ContaId = conta.Id, Descricao = "Crédito de empréstimo", Tipo = TipoTransacao.Emprestimo, Valor = e.ValorSolicitado, DataHora = agora };
            return (emprestimo, transacao);
        });
        return Resposta(resultado);
    }
    public Task<PagamentoParcelaResposta> PagarParcela(int id, int numero) => repositorio.Pagar(id, (emprestimo, conta) =>
    {
        if (emprestimo is null) throw new RegraException("Empréstimo não encontrado.", 404);
        var parcela = emprestimo.Parcelas.SingleOrDefault(p => p.Numero == numero) ?? throw new RegraException("Parcela não encontrada.", 404);
        if (emprestimo.Status != StatusEmprestimo.Aprovado) throw new RegraException("O empréstimo não está em aberto.", 422);
        if (parcela.Status == StatusParcela.Paga) throw new RegraException("Parcela já paga.", 422);
        if (emprestimo.Parcelas.Any(p => p.Numero < numero && p.Status == StatusParcela.Pendente)) throw new RegraException("Pague as parcelas anteriores primeiro.", 422);
        if (conta is null) throw new RegraException("Conta não encontrada.", 404);
        if (!conta.Ativa) throw new RegraException("Conta inativa.", 422);
        if (conta.Saldo < parcela.Valor) throw new RegraException("Saldo insuficiente.", 422);
        conta.Saldo -= parcela.Valor;
        var agora = DateTime.UtcNow; parcela.Status = StatusParcela.Paga; parcela.DataPagamento = agora;
        if (emprestimo.Parcelas.All(p => p.Status == StatusParcela.Paga)) emprestimo.Status = StatusEmprestimo.Quitado;
        var transacao = new Transacao { ContaId = conta.Id, Descricao = $"Pagamento da parcela {numero} do empréstimo {id}", Tipo = TipoTransacao.PagamentoParcela, Valor = parcela.Valor, DataHora = agora };
        return (new PagamentoParcelaResposta(id, numero, parcela.Valor, conta.Saldo, emprestimo.Status, agora), transacao);
    });
    public async Task<EmprestimoResposta> Buscar(int id) => Resposta(await repositorio.Buscar(id) ?? throw new RegraException("Empréstimo não encontrado.", 404));
    public async Task<List<EmprestimoResposta>> Listar(int? clienteId, StatusEmprestimo? status)
    {
        if (status.HasValue && !Enum.IsDefined(status.Value)) throw new RegraException("Status de empréstimo inválido.", 400);
        return (await repositorio.Listar(clienteId, status)).Select(Resposta).ToList();
    }
}

