# BancoAula API — Gestão Bancária

API REST de estudo para cadastro de agências, clientes e contas, movimentações, empréstimos e cartões. Projeto avaliativo individual do módulo de Back-End .NET. Os cartões e valores são fictícios; esta aplicação não se conecta a redes bancárias reais.

## Tecnologias e arquitetura

- C# e .NET 10; ASP.NET Core Web API; Swagger UI e OpenAPI.
- Entity Framework Core 10 e SQL Server; migrations para evolução do banco.
- Controllers recebem requisições; Services validam e aplicam regras; Repositories acessam dados; DTOs definem entradas e saídas; middleware padroniza erros.
- Valores monetários usam `decimal` e `decimal(12,2)` no banco; enums são armazenados como texto. Datas e horários de operações são registrados em UTC; as faturas são agrupadas por mês e ano em UTC.

## Pré-requisitos

- .NET SDK 10.0.401 (definido em `global.json`).
- SQL Server em execução e usuário com permissão para criar banco e tabelas.
- Git e acesso ao NuGet para restaurar pacotes e a ferramenta local `dotnet-ef`.

## Como executar

Na raiz do repositório:

```powershell
git clone https://github.com/suporteknechts-cloud/bancoaula-api.git
cd bancoaula-api
dotnet tool restore
dotnet restore src/BancoAula.Api/BancoAula.Api.csproj
```

A conexão padrão em `src/BancoAula.Api/appsettings.json` usa `localhost\SQLEXPRESS`, autenticação Windows e o banco `BancoAulaProjeto`. Ajuste a conexão para sua instância. Para não gravar credenciais no repositório, use uma variável de ambiente na sessão do PowerShell, por exemplo:

```powershell
$env:ConnectionStrings__Padrao = 'Server=localhost\SQLEXPRESS;Database=BancoAulaProjeto;Trusted_Connection=True;TrustServerCertificate=True'
dotnet ef database update --project src/BancoAula.Api
dotnet run --project src/BancoAula.Api
```

Acesse `http://localhost:5161/swagger` no ambiente Development. Se outra porta for exibida no terminal, use-a. O documento OpenAPI fica em `/openapi/v1.json`. O banco é criado pelas migrations se o usuário tiver permissão. Para gerar novamente o script SQL consolidado:

```powershell
dotnet ef migrations script --project src/BancoAula.Api --output database/001-criacao-inicial.sql
```

O script em `database/` contém as migrations disponíveis; execute-o em um banco apropriado e vazio, sem reaplicá-lo sobre um banco já migrado.

## Endpoints principais

| Recurso | Operações |
| --- | --- |
| Agências | `GET`, `POST /api/agencias`; `GET`, `PUT`, `DELETE /api/agencias/{id}` |
| Clientes | `GET`, `POST /api/clientes`; `GET`, `PUT`, `DELETE /api/clientes/{id}` |
| Contas | `POST`, `GET /api/contas`; `GET /api/contas/{id}`; `PATCH /api/contas/{id}/encerrar` |
| Movimentações | `POST /api/contas/{id}/depositos`, `/saques`; `POST /api/transferencias`; `GET /api/contas/{id}/extrato` |
| Empréstimos | `POST`, `GET /api/emprestimos`; `GET /api/emprestimos/{id}`; `POST /api/emprestimos/{id}/parcelas/{numero}/pagamento` |
| Cartões | `POST /api/cartoes`; `GET /api/cartoes/{id}`; `POST /api/cartoes/{id}/compras`; `GET /api/cartoes/{id}/fatura?mes=11&ano=2026`; `POST /api/cartoes/{id}/faturas/pagamento` |

A listagem de contas aceita `clienteId`, `agenciaId`, `tipo` e `ativa`; o extrato aceita `de`, `ate` (datas `AAAA-MM-DD`, inclusivas) e `tipo`; a listagem de empréstimos aceita `clienteId` e `status`. O Swagger apresenta os modelos das requisições e respostas. Envie valores monetários como números JSON, por exemplo `{"valor":100.00}`.

## Regras principais

- Contas iniciam ativas, com saldo zero. Saques e transferências exigem saldo suficiente; contas inativas não movimentam valores. Transferências registram as duas pontas em uma transação no banco.
- O empréstimo exige saldo em contas ativas de pelo menos 10% do valor solicitado; valores de R$ 500 a R$ 50.000 e de 6 a 48 parcelas. Juros de 2% ao mês pela Tabela Price; as parcelas são pagas em ordem. Pedidos recusados são registrados com motivo.
- Cartões são emitidos para conta corrente ativa, com limite de R$ 500 a R$ 20.000. O número completo não é retornado pela API. Compras reduzem o limite; pagar integralmente a fatura debita a conta e recompõe o limite.
- Uma conta não pode ser encerrada com saldo, empréstimo aprovado em aberto ou cartão ativo. Exclusão de agência ou cliente com vínculos é bloqueada.
- O middleware retorna erros em JSON sem stack trace. Operações financeiras usam transações e bloqueios no SQL Server.

## Exemplo de fluxo no Swagger

1. Cadastre uma agência e um cliente com CPF de 11 dígitos e e-mail válido; anote os IDs.
2. Abra duas contas para o cliente. Deposite na primeira, saque e transfira para a segunda; consulte o extrato.
3. Para solicitar R$ 10.000 em 12 parcelas, mantenha pelo menos R$ 1.000 somados nas contas ativas antes do pedido. Pague a primeira parcela pela rota de pagamento.
4. Emita um cartão para a conta corrente, registre uma compra e pague a fatura do mês/ano UTC da compra, após garantir saldo suficiente.
5. Experimente um saque acima do saldo e uma consulta a ID inexistente para observar os erros tratados.

## Limitações e apresentação

Este é um projeto didático, sem autenticação JWT ou integração com provedores financeiros. O bloqueio/desbloqueio de cartões é funcionalidade bônus ainda não implementada. O vídeo de demonstração deve ser adicionado aqui quando estiver disponível.
