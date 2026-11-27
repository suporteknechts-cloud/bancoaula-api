using BancoAula.Api.Dtos;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[ApiController]
[Route("api/clientes")]
public class ClientesController(IClienteService service) : ControllerBase
{
    [HttpGet, ProducesResponseType(typeof(List<ClienteResposta>), 200)]
    public async Task<ActionResult<List<ClienteResposta>>> Listar() => Ok(await service.Listar());
    [HttpGet("{id:int}"), ProducesResponseType(typeof(ClienteResposta), 200), ProducesResponseType(404)]
    public async Task<ActionResult<ClienteResposta>> Buscar(int id) => Ok(await service.Buscar(id));
    [HttpPost, ProducesResponseType(typeof(ClienteResposta), 201), ProducesResponseType(400), ProducesResponseType(409)]
    public async Task<ActionResult<ClienteResposta>> Criar(ClienteEntrada entrada) { var c = await service.Criar(entrada); return CreatedAtAction(nameof(Buscar), new { id = c.Id }, c); }
    [HttpPut("{id:int}"), ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(409)]
    public async Task<IActionResult> Atualizar(int id, ClienteEntrada entrada) { await service.Atualizar(id, entrada); return NoContent(); }
    [HttpDelete("{id:int}"), ProducesResponseType(204), ProducesResponseType(404), ProducesResponseType(409)]
    public async Task<IActionResult> Remover(int id) { await service.Remover(id); return NoContent(); }
}
