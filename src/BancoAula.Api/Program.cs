using Microsoft.OpenApi;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BancoAula.Api.Repositories;
using BancoAula.Api.Services;
using BancoAula.Api.Middlewares;
using BancoAula.Api.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<BancoContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Padrao")));
builder.Services.AddScoped<IAgenciaRepository, AgenciaRepository>();
builder.Services.AddScoped<IAgenciaService, AgenciaService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();
builder.Services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();
builder.Services.AddScoped<IEmprestimoService, EmprestimoService>();
builder.Services.AddScoped<ICartaoRepository, CartaoRepository>();
builder.Services.AddScoped<ICartaoService, CartaoService>();
var chaveJwt = builder.Configuration["Jwt:Chave"];
if (string.IsNullOrWhiteSpace(chaveJwt) || Encoding.UTF8.GetByteCount(chaveJwt) < 32) throw new InvalidOperationException("Configure Jwt:Chave com pelo menos 32 bytes em user-secrets ou variável de ambiente.");
builder.Services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<BancoContext>();
builder.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true, ValidIssuer = "BancoAula", ValidateAudience = true, ValidAudience = "BancoAula",
    ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt)),
    ValidateLifetime = true, ClockSkew = TimeSpan.Zero
});
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, context, cancellationToken) =>
{
    document.Components ??= new OpenApiComponents();
    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", Description = "Cole o token obtido no login." };
    foreach (var path in document.Paths)
    {
        var protegido = path.Key.StartsWith("/api/cartoes", StringComparison.OrdinalIgnoreCase)
            || path.Key.StartsWith("/api/emprestimos", StringComparison.OrdinalIgnoreCase)
            || path.Key.StartsWith("/api/transferencias", StringComparison.OrdinalIgnoreCase)
            || path.Key.EndsWith("/depositos", StringComparison.OrdinalIgnoreCase)
            || path.Key.EndsWith("/saques", StringComparison.OrdinalIgnoreCase)
            || path.Key.EndsWith("/extrato", StringComparison.OrdinalIgnoreCase);
        if (!protegido || path.Value.Operations is null) continue;
        foreach (var operation in path.Value.Operations.Values)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>() });
        }
    }
    return Task.CompletedTask;
}));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "BancoAula API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


