using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BancoAula.Api.Dtos;
using BancoAula.Api.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
namespace BancoAula.Api.Services;
public interface IAutenticacaoService
{
    Task Cadastrar(CredenciaisEntrada entrada);
    Task<TokenResposta> Login(CredenciaisEntrada entrada);
}
public class AutenticacaoService(UserManager<IdentityUser> usuarios, IConfiguration configuracao) : IAutenticacaoService
{
    public async Task Cadastrar(CredenciaisEntrada entrada)
    {
        var usuario = new IdentityUser { UserName = entrada.Email.Trim(), Email = entrada.Email.Trim() };
        var resultado = await usuarios.CreateAsync(usuario, entrada.Senha);
        if (!resultado.Succeeded) throw new RegraException(string.Join(" ", resultado.Errors.Select(e => e.Description)), 400);
    }
    public async Task<TokenResposta> Login(CredenciaisEntrada entrada)
    {
        var usuario = await usuarios.FindByEmailAsync(entrada.Email.Trim());
        if (usuario is null || !await usuarios.CheckPasswordAsync(usuario, entrada.Senha)) throw new RegraException("Credenciais inválidas.", 401);
        var chave = configuracao["Jwt:Chave"] ?? throw new InvalidOperationException("Chave JWT não configurada.");
        var expira = DateTime.UtcNow.AddHours(1);
        var token = new JwtSecurityToken(issuer: "BancoAula", audience: "BancoAula", claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, usuario.Id) }, expires: expira, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)), SecurityAlgorithms.HmacSha256));
        return new TokenResposta(new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
