using BancoAula.Api.Dtos;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[ApiController]
[Route("api/contas")]
public class ContasController(IContaService service) : ControllerBase
{
    [HttpGet, ProducesResponseType(typeof(List<ContaResposta>), 200)]
    public async Task<ActionResult<List<ContaResposta>>> Listar([FromQuery] int? clienteId, [FromQuery] int? agenciaId, [FromQuery] TipoConta? tipo, [FromQuery] bool? ativa) => Ok(await service.Listar(clienteId, agenciaId, tipo, ativa));
    [HttpGet("{id:int}"), ProducesResponseType(typeof(ContaResposta), 200), ProducesResponseType(404)]
    public async Task<ActionResult<ContaResposta>> Buscar(int id) => Ok(await service.Buscar(id));
    [HttpPost, ProducesResponseType(typeof(ContaResposta), 201), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(409)]
    public async Task<ActionResult<ContaResposta>> Criar(ContaEntrada entrada) { var c = await service.Criar(entrada); return CreatedAtAction(nameof(Buscar), new { id = c.Id }, c); }
    [HttpPatch("{id:int}/encerrar"), ProducesResponseType(204), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<IActionResult> Encerrar(int id) { await service.Encerrar(id); return NoContent(); }
}
