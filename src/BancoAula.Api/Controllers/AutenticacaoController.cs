using BancoAula.Api.Dtos;
using BancoAula.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoAula.Api.Controllers;
[ApiController]
[Route("api/autenticacao")]
public class AutenticacaoController(IAutenticacaoService service) : ControllerBase
{
    [HttpPost("cadastro"), ProducesResponseType(201), ProducesResponseType(400)]
    public async Task<IActionResult> Cadastrar(CredenciaisEntrada entrada) { await service.Cadastrar(entrada); return StatusCode(201); }
    [HttpPost("login"), ProducesResponseType(typeof(TokenResposta), 200), ProducesResponseType(401)]
    public async Task<ActionResult<TokenResposta>> Login(CredenciaisEntrada entrada) => Ok(await service.Login(entrada));
}
