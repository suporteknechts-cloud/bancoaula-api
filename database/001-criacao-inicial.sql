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

