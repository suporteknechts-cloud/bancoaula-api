using BancoAula.Api.Dtos;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[ApiController]
[Route("api/cartoes")]
public class CartoesController(ICartaoService service) : ControllerBase
{
    [HttpPost, ProducesResponseType(typeof(CartaoResposta), 201), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<CartaoResposta>> Emitir(CartaoEntrada entrada) { var c = await service.Emitir(entrada); return CreatedAtAction(nameof(Buscar), new { id = c.Id }, c); }
    [HttpGet("{id:int}"), ProducesResponseType(typeof(CartaoResposta), 200), ProducesResponseType(404)]
    public async Task<ActionResult<CartaoResposta>> Buscar(int id) => Ok(await service.Buscar(id));
}
