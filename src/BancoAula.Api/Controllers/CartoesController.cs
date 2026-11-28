using Microsoft.AspNetCore.Authorization;
using BancoAula.Api.Dtos;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[Authorize]
[ApiController]
[Route("api/cartoes")]
public class CartoesController(ICartaoService service) : ControllerBase
{
    [HttpPost, ProducesResponseType(typeof(CartaoResposta), 201), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<CartaoResposta>> Emitir(CartaoEntrada entrada) { var c = await service.Emitir(entrada); return CreatedAtAction(nameof(Buscar), new { id = c.Id }, c); }
    [HttpPatch("{id:int}/bloquear"), ProducesResponseType(204), ProducesResponseType(404)]
    public async Task<IActionResult> Bloquear(int id) { await service.AlterarStatus(id, true); return NoContent(); }
    [HttpPatch("{id:int}/desbloquear"), ProducesResponseType(204), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<IActionResult> Desbloquear(int id) { await service.AlterarStatus(id, false); return NoContent(); }
    [HttpPost("{id:int}/compras"), ProducesResponseType(typeof(CompraResposta), 200), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<CompraResposta>> Comprar(int id, CompraEntrada entrada) => Ok(await service.Comprar(id, entrada));
    [HttpGet("{id:int}/fatura"), ProducesResponseType(typeof(FaturaResposta), 200), ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<ActionResult<FaturaResposta>> Fatura(int id, [FromQuery] int mes, [FromQuery] int ano) => Ok(await service.Fatura(id, mes, ano));
    [HttpPost("{id:int}/faturas/pagamento"), ProducesResponseType(typeof(PagamentoFaturaResposta), 200), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<PagamentoFaturaResposta>> PagarFatura(int id, PagamentoFaturaEntrada entrada) => Ok(await service.PagarFatura(id, entrada));
    [HttpGet("{id:int}"), ProducesResponseType(typeof(CartaoResposta), 200), ProducesResponseType(404)]
    public async Task<ActionResult<CartaoResposta>> Buscar(int id) => Ok(await service.Buscar(id));
}
