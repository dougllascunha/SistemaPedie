USE [dbpedie]
GO

-- =========================================================================
-- 1. CRIAÇÃO DAS TABELAS (DDL)
-- =========================================================================

/****** Objeto:  Table [dbo].[Categoria] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.Categoria', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Categoria](
        [idCategoria] [int] IDENTITY(1,1) NOT NULL,
        [nome] [varchar](50) NOT NULL,
        PRIMARY KEY CLUSTERED ([idCategoria] ASC)
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[Cliente] ******/
IF OBJECT_ID('dbo.Cliente', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Cliente](
        [idCliente] [int] IDENTITY(1,1) NOT NULL,
        [nome] [varchar](150) NOT NULL,
        [nomeSocial] [varchar](150) NULL,
        [email] [varchar](150) NOT NULL,
        [telefone] [varchar](15) NULL,
        [senhaAcesso] [varchar](255) NOT NULL,
        [perfil] [varchar](20) NOT NULL DEFAULT ('Cliente'),
        [dataCadastro] [datetime] NOT NULL DEFAULT (GETDATE()),
        PRIMARY KEY CLUSTERED ([idCliente] ASC),
        UNIQUE NONCLUSTERED ([email] ASC)
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[Situacao] ******/
IF OBJECT_ID('dbo.Situacao', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Situacao](
        [idSituacao] [int] IDENTITY(1,1) NOT NULL,
        [descricao] [varchar](50) NOT NULL,
        PRIMARY KEY CLUSTERED ([idSituacao] ASC),
        UNIQUE NONCLUSTERED ([descricao] ASC)
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[Produto] ******/
IF OBJECT_ID('dbo.Produto', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Produto](
        [idProduto] [int] IDENTITY(1,1) NOT NULL,
        [nome] [varchar](150) NOT NULL,
        [descricao] [varchar](255) NULL,
        [preco] [decimal](10, 2) NOT NULL,
        [disponivel] [bit] NOT NULL DEFAULT ((1)),
        [imagemUrl] [varchar](500) NULL,
        [idCategoria] [int] NOT NULL,
        PRIMARY KEY CLUSTERED ([idProduto] ASC),
        CONSTRAINT [FK_Produto_Categoria] FOREIGN KEY([idCategoria]) REFERENCES [dbo].[Categoria] ([idCategoria])
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[Endereco] ******/
IF OBJECT_ID('dbo.Endereco', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Endereco](
        [idEndereco] [int] IDENTITY(1,1) NOT NULL,
        [idCliente] [int] NOT NULL,
        [cep] [varchar](10) NULL,
        [rua] [varchar](150) NULL,
        [numero] [varchar](10) NULL,
        [bairro] [varchar](100) NULL,
        [cidade] [varchar](100) NULL,
        [uf] [char](2) NULL,
        [complemento] [varchar](30) NULL,
        PRIMARY KEY CLUSTERED ([idEndereco] ASC),
        CONSTRAINT [FK_Endereco_Cliente] FOREIGN KEY([idCliente]) REFERENCES [dbo].[Cliente] ([idCliente])
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[Pedido] ******/
IF OBJECT_ID('dbo.Pedido', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Pedido](
        [idPedido] [int] IDENTITY(1,1) NOT NULL,
        [idCliente] [int] NOT NULL,
        [idSituacao] [int] NOT NULL,
        [dataPedido] [datetime] NOT NULL DEFAULT (GETDATE()),
        [valorTotal] [decimal](10, 2) NOT NULL,
        PRIMARY KEY CLUSTERED ([idPedido] ASC),
        CONSTRAINT [FK_Pedido_Cliente] FOREIGN KEY([idCliente]) REFERENCES [dbo].[Cliente] ([idCliente]),
        CONSTRAINT [FK_Pedido_Situacao] FOREIGN KEY([idSituacao]) REFERENCES [dbo].[Situacao] ([idSituacao])
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[ItemPedido] ******/
IF OBJECT_ID('dbo.ItemPedido', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ItemPedido](
        [idItemPedido] [int] IDENTITY(1,1) NOT NULL,
        [idPedido] [int] NOT NULL,
        [idProduto] [int] NOT NULL,
        [quantidade] [int] NOT NULL,
        [precoUnitario] [decimal](10, 2) NOT NULL,
        PRIMARY KEY CLUSTERED ([idItemPedido] ASC),
        CONSTRAINT [FK_ItemPedido_Pedido] FOREIGN KEY([idPedido]) REFERENCES [dbo].[Pedido] ([idPedido]),
        CONSTRAINT [FK_ItemPedido_Produto] FOREIGN KEY([idProduto]) REFERENCES [dbo].[Produto] ([idProduto])
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[Pagamento] ******/
IF OBJECT_ID('dbo.Pagamento', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Pagamento](
        [idPagamento] [int] IDENTITY(1,1) NOT NULL,
        [idPedido] [int] NOT NULL,
        [tipoPagamento] [varchar](30) NOT NULL,
        [valor] [decimal](10, 2) NOT NULL,
        [dataPagamento] [datetime] NOT NULL DEFAULT (GETDATE()),
        PRIMARY KEY CLUSTERED ([idPagamento] ASC),
        CONSTRAINT [FK_Pagamento_Pedido] FOREIGN KEY([idPedido]) REFERENCES [dbo].[Pedido] ([idPedido])
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[Promocao] ******/
IF OBJECT_ID('dbo.Promocao', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Promocao](
        [idPromocao] [int] IDENTITY(1,1) NOT NULL,
        [titulo] [varchar](150) NOT NULL,
        [descricao] [varchar](255) NULL,
        [percentualDesconto] [decimal](5, 2) NULL,
        [dataInicio] [date] NULL,
        [dataFim] [date] NULL,
        PRIMARY KEY CLUSTERED ([idPromocao] ASC)
    ) ON [PRIMARY]
END
GO

/****** Objeto:  Table [dbo].[HistoricoStatusPedido] (Auditoria para Trigger) ******/
IF OBJECT_ID('dbo.HistoricoStatusPedido', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[HistoricoStatusPedido](
        [idHistorico] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [idPedido] [int] NOT NULL,
        [idSituacaoAnterior] [int] NULL,
        [idSituacaoNova] [int] NOT NULL,
        [dataAlteracao] [datetime] NOT NULL DEFAULT (GETDATE())
    ) ON [PRIMARY]
END
GO

-- =========================================================================
-- 2. STORED PROCEDURES (PROCEDIMENTOS ARMAZENADOS EM T-SQL COM SHA-256)
-- =========================================================================

-- Procedure 1: Cadastra cliente convertendo a senha automaticamente para Hash SHA-256
CREATE OR ALTER PROCEDURE [dbo].[sp_CadastrarCliente]
    @nome VARCHAR(150),
    @nomeSocial VARCHAR(150) = NULL,
    @email VARCHAR(150),
    @telefone VARCHAR(15) = NULL,
    @senhaAcesso VARCHAR(255),
    @perfil VARCHAR(20) = 'Cliente'
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Cliente] WHERE [email] = @email)
    BEGIN
        RAISERROR('Este e-mail já está cadastrado no sistema.', 16, 1);
        RETURN;
    END

    -- Converte a senha em texto puro para Hash Hexadecimal SHA-256 (64 caracteres)
    DECLARE @senhaHash VARCHAR(64);
    SET @senhaHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(255), @senhaAcesso)), 2);

    INSERT INTO [dbo].[Cliente] ([nome], [nomeSocial], [email], [telefone], [senhaAcesso], [perfil], [dataCadastro])
    VALUES (@nome, @nomeSocial, @email, @telefone, @senhaHash, @perfil, GETDATE());
END
GO

-- Procedure 2: Redefine a senha do cliente aplicando criptografia SHA-256
CREATE OR ALTER PROCEDURE [dbo].[sp_RedefinirSenhaCliente]
    @idCliente INT,
    @novaSenha VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @senhaHash VARCHAR(64);
    SET @senhaHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(255), @novaSenha)), 2);

    UPDATE [dbo].[Cliente]
    SET [senhaAcesso] = @senhaHash
    WHERE [idCliente] = @idCliente;
END
GO

-- Procedure 3: Atualiza o status de um pedido validando a existência da situação
CREATE OR ALTER PROCEDURE [dbo].[sp_AtualizarStatusPedido]
    @idPedido INT,
    @idNovaSituacao INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Situacao] WHERE [idSituacao] = @idNovaSituacao)
    BEGIN
        RAISERROR('A situação informada não existe no banco de dados.', 16, 1);
        RETURN;
    END

    UPDATE [dbo].[Pedido]
    SET [idSituacao] = @idNovaSituacao
    WHERE [idPedido] = @idPedido;
END
GO

-- Procedure 4: Consulta os pedidos de um cliente com descrição do status para sincronização Web/Mobile
CREATE OR ALTER PROCEDURE [dbo].[sp_ListarPedidosCliente]
    @idCliente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.[idPedido],
        p.[dataPedido],
        p.[valorTotal],
        p.[idSituacao],
        s.[descricao] AS [descricaoSituacao]
    FROM [dbo].[Pedido] p
    INNER JOIN [dbo].[Situacao] s ON p.[idSituacao] = s.[idSituacao]
    WHERE p.[idCliente] = @idCliente
    ORDER BY p.[dataPedido] DESC;
END
GO

-- =========================================================================
-- 3. TRIGGERS (GATILHOS AUTOMÁTICOS EM T-SQL)
-- =========================================================================

-- Trigger 1: Registra automaticamente no histórico toda vez que o status de um pedido é alterado
CREATE OR ALTER TRIGGER [dbo].[trg_AuditoriaStatusPedido]
ON [dbo].[Pedido]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE([idSituacao])
    BEGIN
        INSERT INTO [dbo].[HistoricoStatusPedido] ([idPedido], [idSituacaoAnterior], [idSituacaoNova], [dataAlteracao])
        SELECT 
            i.[idPedido],
            d.[idSituacao],
            i.[idSituacao],
            GETDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.[idPedido] = d.[idPedido]
        WHERE i.[idSituacao] <> d.[idSituacao];
    END
END
GO

-- Trigger 2: Impede o cadastro ou atualização de produtos com preço menor ou igual a zero
CREATE OR ALTER TRIGGER [dbo].[trg_ValidarPrecoProduto]
ON [dbo].[Produto]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE [preco] <= 0)
    BEGIN
        RAISERROR('O preço do produto deve ser maior que zero.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO

-- =========================================================================
-- 4. CARGA INICIAL DE DADOS (DML - INSERT INTO)
-- =========================================================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[Categoria])
BEGIN
    SET IDENTITY_INSERT [dbo].[Categoria] ON;
    INSERT INTO [dbo].[Categoria] ([idCategoria], [nome]) VALUES
    (1, 'Bolos'),
    (2, 'Tortas'),
    (3, 'Doces Individuais'),
    (4, 'Sobremesas');
    SET IDENTITY_INSERT [dbo].[Categoria] OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Situacao])
BEGIN
    SET IDENTITY_INSERT [dbo].[Situacao] ON;
    INSERT INTO [dbo].[Situacao] ([idSituacao], [descricao]) VALUES
    (1, 'Aguardando Pagamento'),
    (2, 'Preparando'),
    (3, 'Pronto para Retirada na Loja'),
    (4, 'Entregue'),
    (5, 'Saiu para Entrega'),
    (6, 'Cancelado');
    SET IDENTITY_INSERT [dbo].[Situacao] OFF;
END
ELSE
BEGIN
    UPDATE [dbo].[Situacao] SET [descricao] = 'Pronto para Retirada na Loja' WHERE [idSituacao] = 3;
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Situacao] WHERE [idSituacao] = 5)
    BEGIN
        SET IDENTITY_INSERT [dbo].[Situacao] ON;
        INSERT INTO [dbo].[Situacao] ([idSituacao], [descricao]) VALUES (5, 'Saiu para Entrega');
        SET IDENTITY_INSERT [dbo].[Situacao] OFF;
    END
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Situacao] WHERE [idSituacao] = 6)
    BEGIN
        SET IDENTITY_INSERT [dbo].[Situacao] ON;
        INSERT INTO [dbo].[Situacao] ([idSituacao], [descricao]) VALUES (6, 'Cancelado');
        SET IDENTITY_INSERT [dbo].[Situacao] OFF;
    END
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Produto])
BEGIN
    INSERT INTO [dbo].[Produto] ([nome], [descricao], [preco], [disponivel], [imagemUrl], [idCategoria]) VALUES
    ('Bolo de Cenoura', 'Bolo de cenoura fofinho com cobertura generosa de brigadeiro belga.', 22.00, 1, '/imagens/BoloCenoura.jpg', 1),
    ('Bolo de Chocolate', 'Bolo de chocolate 50% cacau com recheio e cobertura cremosa.', 25.00, 1, '/imagens/BoloChocolate.jpg', 1),
    ('Bolo Red Velvet', 'Bolo aveludado vermelho com recheio autêntico de cream cheese.', 28.00, 1, '/imagens/BoloRedVelvet.jpg', 1),
    ('Bolo de Churros', 'Massa com toque de canela e cobertura de doce de leite artesanal.', 24.00, 1, '/imagens/BoloChurros.webp', 1),
    ('Bolo de Maracujá', 'Bolo macio com mousse aerado e calda natural de maracujá.', 23.00, 1, '/imagens/BoloMaracuja.jpg', 1),
    ('Bolo de Pote Ninho', 'Camadas de massa branca, creme de Leite Ninho e morangos frescos.', 15.00, 1, '/imagens/BoloPoteNinho.webp', 4),
    ('Torta Holandesa', 'Base de biscoito amanteigado, creme holandês e ganache meio amargo.', 18.00, 1, '/imagens/TortaHolandesa.webp', 2),
    ('Torta de Limão', 'Massa crocante recheada com creme de limão e merengue maçaricado.', 16.00, 1, '/imagens/TortaLimao.webp', 2),
    ('Torta de Morango', 'Torta cremosa de baunilha coberta com morangos selecionados.', 19.00, 1, '/imagens/TortaMorango.webp', 2),
    ('Cheesecake de Frutas Vermelhas', 'Cheesecake clássico estilo NY com calda artesanal de frutas vermelhas.', 21.00, 1, '/imagens/Cheesecake.webp', 2),
    ('Brigadeiro Gourmet', 'Unidade de brigadeiro gourmet feito com chocolate nobre e granulado belga.', 6.50, 1, '/imagens/BrigadeiroGourmet.webp', 3),
    ('Brownie com Nozes', 'Brownie denso de chocolate meio amargo com pedaços de nozes.', 14.00, 1, '/imagens/BrownieNozes.webp', 3),
    ('Coxinha de Morango', 'Morango inteiro fresco envolvido em brigadeiro tradicional.', 10.00, 1, '/imagens/CoxinhaMorango.webp', 3),
    ('Macarons Parisienses', 'Porção de macarons crocantes por fora e macios por dentro.', 18.00, 1, '/imagens/Macarons.jpg', 3),
    ('Copo da Felicidade', 'Camadas generosas de brownie, creme de avelã, Ninho e morangos.', 22.00, 1, '/imagens/CopoFelicidade.webp', 4),
    ('Pudim de Leite Condensado', 'Pudim lisinho tradicional com calda de caramelo dourado.', 14.00, 1, '/imagens/Pudim.webp', 4);
END
GO

-- Clientes iniciais já cadastrados via Stored Procedure com senha criptografada em SHA-256 ('123456')
IF NOT EXISTS (SELECT 1 FROM [dbo].[Cliente])
BEGIN
    EXEC [dbo].[sp_CadastrarCliente]
        @nome = 'Administrador Pediê',
        @nomeSocial = 'Admin Pediê',
        @email = 'admin@pedie.com',
        @telefone = '(11) 99999-9999',
        @senhaAcesso = '123456',
        @perfil = 'Admin';

    EXEC [dbo].[sp_CadastrarCliente]
        @nome = 'Cliente Teste',
        @nomeSocial = 'Cliente',
        @email = 'cliente@pedie.com',
        @telefone = '(11) 98888-8888',
        @senhaAcesso = '123456',
        @perfil = 'Cliente';
END
ELSE
BEGIN
    -- Atualiza senhas que ainda não tenham 64 caracteres (texto puro antigo) para SHA-256
    UPDATE [dbo].[Cliente]
    SET [senhaAcesso] = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(255), [senhaAcesso])), 2)
    WHERE LEN([senhaAcesso]) <> 64;
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Promocao])
BEGIN
    INSERT INTO [dbo].[Promocao] ([titulo], [descricao], [percentualDesconto], [dataInicio], [dataFim]) VALUES
    ('Semana do Chocolate Pediê', 'Aproveite desconto especial em todos os bolos e brownies de chocolate!', 15.00, DATEADD(day, -5, GETDATE()), DATEADD(day, 30, GETDATE())),
    ('Festival de Tortas Artesanais', 'Nossas tortas clássicas com desconto imperdível para adoçar seu dia.', 20.00, DATEADD(day, -2, GETDATE()), DATEADD(day, 15, GETDATE()));
END
GO
