using Microsoft.AspNetCore.Authorization;
using BancoAula.Api.Dtos;
using BancoAula.Api.Models.Entities;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[Authorize]
[ApiController]
[Route("api")]
public class TransferenciasController(ITransferenciaService service) : ControllerBase
{
    [HttpPost("transferencias"), ProducesResponseType(typeof(TransferenciaResposta), 200), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<TransferenciaResposta>> Transferir(TransferenciaEntrada entrada) => Ok(await service.Transferir(entrada));
    [HttpGet("contas/{id:int}/extrato"), ProducesResponseType(typeof(ExtratoResposta), 200), ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<ActionResult<ExtratoResposta>> Extrato(int id, [FromQuery] DateOnly? de, [FromQuery] DateOnly? ate, [FromQuery] TipoTransacao? tipo) => Ok(await service.Extrato(id, de, ate, tipo));
}
