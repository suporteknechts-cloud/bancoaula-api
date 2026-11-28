using BancoAula.Api.Dtos;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[ApiController]
[Route("api/emprestimos")]
public class EmprestimosController(IEmprestimoService service) : ControllerBase
{
    [HttpPost, ProducesResponseType(typeof(EmprestimoResposta), 201), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<EmprestimoResposta>> Solicitar(EmprestimoEntrada entrada) { var e = await service.Solicitar(entrada); return CreatedAtAction(nameof(Buscar), new { id = e.Id }, e); }
    [HttpPost("{id:int}/parcelas/{numero:int}/pagamento"), ProducesResponseType(typeof(PagamentoParcelaResposta), 200), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<PagamentoParcelaResposta>> PagarParcela(int id, int numero) => Ok(await service.PagarParcela(id, numero));
    [HttpGet("{id:int}"), ProducesResponseType(typeof(EmprestimoResposta), 200), ProducesResponseType(404)]
    public async Task<ActionResult<EmprestimoResposta>> Buscar(int id) => Ok(await service.Buscar(id));
    [HttpGet, ProducesResponseType(typeof(List<EmprestimoResposta>), 200)]
    public async Task<ActionResult<List<EmprestimoResposta>>> Listar([FromQuery] int? clienteId, [FromQuery] StatusEmprestimo? status) => Ok(await service.Listar(clienteId, status));
}

