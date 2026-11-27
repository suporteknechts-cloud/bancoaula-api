using BancoAula.Api.Dtos;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[ApiController]
[Route("api/agencias")]
public class AgenciasController(IAgenciaService service) : ControllerBase
{
    [HttpGet, ProducesResponseType(typeof(List<AgenciaResposta>), 200)]
    public async Task<ActionResult<List<AgenciaResposta>>> Listar() => Ok(await service.Listar());
    [HttpGet("{id:int}"), ProducesResponseType(typeof(AgenciaResposta), 200), ProducesResponseType(404)]
    public async Task<ActionResult<AgenciaResposta>> Buscar(int id) => Ok(await service.Buscar(id));
    [HttpPost, ProducesResponseType(typeof(AgenciaResposta), 201), ProducesResponseType(400)]
    public async Task<ActionResult<AgenciaResposta>> Criar(AgenciaEntrada entrada) { var a = await service.Criar(entrada); return CreatedAtAction(nameof(Buscar), new { id = a.Id }, a); }
    [HttpPut("{id:int}"), ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<IActionResult> Atualizar(int id, AgenciaEntrada entrada) { await service.Atualizar(id, entrada); return NoContent(); }
    [HttpDelete("{id:int}"), ProducesResponseType(204), ProducesResponseType(404), ProducesResponseType(409)]
    public async Task<IActionResult> Remover(int id) { await service.Remover(id); return NoContent(); }
}
