IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Agencias] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Cidade] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Agencias] PRIMARY KEY ([Id])
);

CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Cpf] nvarchar(11) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Cidade] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);

CREATE TABLE [Contas] (
    [Id] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [AgenciaId] int NOT NULL,
    [Numero] nvarchar(50) NOT NULL,
    [Tipo] nvarchar(30) NOT NULL,
    [Saldo] decimal(12,2) NOT NULL,
    [Ativa] bit NOT NULL,
    CONSTRAINT [PK_Contas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Contas_Agencias_AgenciaId] FOREIGN KEY ([AgenciaId]) REFERENCES [Agencias] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Contas_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Transacoes] (
    [Id] int NOT NULL IDENTITY,
    [ContaId] int NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    [Tipo] nvarchar(40) NOT NULL,
    [Valor] decimal(12,2) NOT NULL,
    [DataHora] datetime2 NOT NULL,
    CONSTRAINT [PK_Transacoes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transacoes_Contas_ContaId] FOREIGN KEY ([ContaId]) REFERENCES [Contas] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Clientes_Cpf] ON [Clientes] ([Cpf]);

CREATE INDEX [IX_Contas_AgenciaId] ON [Contas] ([AgenciaId]);

CREATE INDEX [IX_Contas_ClienteId] ON [Contas] ([ClienteId]);

CREATE UNIQUE INDEX [IX_Contas_Numero] ON [Contas] ([Numero]);

CREATE INDEX [IX_Transacoes_ContaId] ON [Transacoes] ([ContaId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261127230808_CriacaoInicial', N'10.0.12');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Emprestimos] (
    [Id] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [ContaId] int NOT NULL,
    [ValorSolicitado] decimal(12,2) NOT NULL,
    [TaxaJurosMensal] decimal(8,6) NOT NULL,
    [QuantidadeParcelas] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [MotivoRecusa] nvarchar(max) NULL,
    [DataSolicitacao] datetime2 NOT NULL,
    CONSTRAINT [PK_Emprestimos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Emprestimos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Emprestimos_Contas_ContaId] FOREIGN KEY ([ContaId]) REFERENCES [Contas] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Parcelas] (
    [Id] int NOT NULL IDENTITY,
    [EmprestimoId] int NOT NULL,
    [Numero] int NOT NULL,
    [Valor] decimal(12,2) NOT NULL,
    [DataVencimento] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [DataPagamento] datetime2 NULL,
    CONSTRAINT [PK_Parcelas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Parcelas_Emprestimos_EmprestimoId] FOREIGN KEY ([EmprestimoId]) REFERENCES [Emprestimos] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Emprestimos_ClienteId] ON [Emprestimos] ([ClienteId]);

CREATE INDEX [IX_Emprestimos_ContaId] ON [Emprestimos] ([ContaId]);

CREATE UNIQUE INDEX [IX_Parcelas_EmprestimoId_Numero] ON [Parcelas] ([EmprestimoId], [Numero]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261127235001_AdicionaEmprestimos', N'10.0.12');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Cartoes] (
    [Id] int NOT NULL IDENTITY,
    [ContaId] int NOT NULL,
    [Numero] nvarchar(16) NOT NULL,
    [NomeImpresso] nvarchar(100) NOT NULL,
    [Limite] decimal(12,2) NOT NULL,
    [LimiteDisponivel] decimal(12,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [DataEmissao] datetime2 NOT NULL,
    CONSTRAINT [PK_Cartoes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cartoes_Contas_ContaId] FOREIGN KEY ([ContaId]) REFERENCES [Contas] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [ComprasCartao] (
    [Id] int NOT NULL IDENTITY,
    [CartaoId] int NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    [Valor] decimal(12,2) NOT NULL,
    [DataCompra] datetime2 NOT NULL,
    [Paga] bit NOT NULL,
    CONSTRAINT [PK_ComprasCartao] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ComprasCartao_Cartoes_CartaoId] FOREIGN KEY ([CartaoId]) REFERENCES [Cartoes] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Cartoes_ContaId] ON [Cartoes] ([ContaId]);

CREATE UNIQUE INDEX [IX_Cartoes_Numero] ON [Cartoes] ([Numero]);

CREATE INDEX [IX_ComprasCartao_CartaoId] ON [ComprasCartao] ([CartaoId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261128001355_AdicionaCartoes', N'10.0.12');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261128010215_AdicionaIdentity', N'10.0.12');

COMMIT;
GO

