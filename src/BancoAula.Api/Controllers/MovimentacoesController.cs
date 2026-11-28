using Microsoft.AspNetCore.Authorization;
using BancoAula.Api.Dtos;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[Authorize]
[ApiController]
[Route("api/contas/{id:int}")]
public class MovimentacoesController(IMovimentacaoService service) : ControllerBase
{
    [HttpPost("depositos"), ProducesResponseType(typeof(MovimentacaoResposta), 200), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<MovimentacaoResposta>> Depositar(int id, MovimentacaoEntrada entrada) => Ok(await service.Depositar(id, entrada.Valor));
    [HttpPost("saques"), ProducesResponseType(typeof(MovimentacaoResposta), 200), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(422)]
    public async Task<ActionResult<MovimentacaoResposta>> Sacar(int id, MovimentacaoEntrada entrada) => Ok(await service.Sacar(id, entrada.Valor));
}
