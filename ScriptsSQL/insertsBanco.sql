USE [dbpedie];
GO

-- =========================================================================
-- 1. CRIAÇÃO DAS TABELAS E PROCEDURES (Caso ainda não existam no dbpedie)
-- =========================================================================

IF OBJECT_ID('dbo.Categoria', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Categoria](
        [idCategoria] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [nome] [varchar](50) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Cliente', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Cliente](
        [idCliente] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [nome] [varchar](150) NOT NULL,
        [nomeSocial] [varchar](150) NULL,
        [email] [varchar](150) NOT NULL UNIQUE,
        [telefone] [varchar](15) NULL,
        [senhaAcesso] [varchar](255) NOT NULL,
        [perfil] [varchar](20) NOT NULL DEFAULT ('Cliente'),
        [dataCadastro] [datetime] NOT NULL DEFAULT (GETDATE())
    );
END
GO

IF OBJECT_ID('dbo.Situacao', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Situacao](
        [idSituacao] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [descricao] [varchar](50) NOT NULL UNIQUE
    );
END
GO

IF OBJECT_ID('dbo.Produto', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Produto](
        [idProduto] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [nome] [varchar](150) NOT NULL,
        [descricao] [varchar](255) NULL,
        [preco] [decimal](10, 2) NOT NULL,
        [disponivel] [bit] NOT NULL DEFAULT ((1)),
        [imagemUrl] [varchar](500) NULL,
        [idCategoria] [int] NOT NULL,
        CONSTRAINT [FK_Produto_Categoria] FOREIGN KEY([idCategoria]) REFERENCES [dbo].[Categoria] ([idCategoria])
    );
END
GO

IF OBJECT_ID('dbo.Endereco', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Endereco](
        [idEndereco] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [idCliente] [int] NOT NULL,
        [cep] [varchar](10) NULL,
        [rua] [varchar](150) NULL,
        [numero] [varchar](10) NULL,
        [bairro] [varchar](100) NULL,
        [cidade] [varchar](100) NULL,
        [uf] [char](2) NULL,
        [complemento] [varchar](30) NULL,
        CONSTRAINT [FK_Endereco_Cliente] FOREIGN KEY([idCliente]) REFERENCES [dbo].[Cliente] ([idCliente])
    );
END
GO

IF OBJECT_ID('dbo.Pedido', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Pedido](
        [idPedido] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [idCliente] [int] NOT NULL,
        [idSituacao] [int] NOT NULL,
        [dataPedido] [datetime] NOT NULL DEFAULT (GETDATE()),
        [valorTotal] [decimal](10, 2) NOT NULL,
        CONSTRAINT [FK_Pedido_Cliente] FOREIGN KEY([idCliente]) REFERENCES [dbo].[Cliente] ([idCliente]),
        CONSTRAINT [FK_Pedido_Situacao] FOREIGN KEY([idSituacao]) REFERENCES [dbo].[Situacao] ([idSituacao])
    );
END
GO

IF OBJECT_ID('dbo.ItemPedido', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ItemPedido](
        [idItemPedido] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [idPedido] [int] NOT NULL,
        [idProduto] [int] NOT NULL,
        [quantidade] [int] NOT NULL,
        [precoUnitario] [decimal](10, 2) NOT NULL,
        CONSTRAINT [FK_ItemPedido_Pedido] FOREIGN KEY([idPedido]) REFERENCES [dbo].[Pedido] ([idPedido]),
        CONSTRAINT [FK_ItemPedido_Produto] FOREIGN KEY([idProduto]) REFERENCES [dbo].[Produto] ([idProduto])
    );
END
GO

IF OBJECT_ID('dbo.Pagamento', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Pagamento](
        [idPagamento] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [idPedido] [int] NOT NULL,
        [tipoPagamento] [varchar](30) NOT NULL,
        [valor] [decimal](10, 2) NOT NULL,
        [dataPagamento] [datetime] NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [FK_Pagamento_Pedido] FOREIGN KEY([idPedido]) REFERENCES [dbo].[Pedido] ([idPedido])
    );
END
GO

IF OBJECT_ID('dbo.Promocao', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Promocao](
        [idPromocao] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [titulo] [varchar](150) NOT NULL,
        [descricao] [varchar](255) NULL,
        [percentualDesconto] [decimal](5, 2) NULL,
        [dataInicio] [date] NULL,
        [dataFim] [date] NULL
    );
END
GO

-- Stored Procedures de Criptografia de Senha em SHA-256
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

    DECLARE @senhaHash VARCHAR(64);
    SET @senhaHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(255), @senhaAcesso)), 2);

    INSERT INTO [dbo].[Cliente] ([nome], [nomeSocial], [email], [telefone], [senhaAcesso], [perfil], [dataCadastro])
    VALUES (@nome, @nomeSocial, @email, @telefone, @senhaHash, @perfil, GETDATE());
END
GO

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

-- =========================================================================
-- 2. INSERÇÃO DOS DADOS INICIAIS (INSERT INTO)
-- =========================================================================

-- Categorias
IF NOT EXISTS (SELECT 1 FROM Categoria)
BEGIN
    SET IDENTITY_INSERT Categoria ON;
    INSERT INTO Categoria (idCategoria, nome) VALUES
    (1, 'Bolos'),
    (2, 'Tortas'),
    (3, 'Doces Individuais'),
    (4, 'Sobremesas');
    SET IDENTITY_INSERT Categoria OFF;
END
GO

-- Situações de Pedido
IF NOT EXISTS (SELECT 1 FROM Situacao)
BEGIN
    SET IDENTITY_INSERT Situacao ON;
    INSERT INTO Situacao (idSituacao, descricao) VALUES
    (1, 'Aguardando Pagamento'),
    (2, 'Preparando'),
    (3, 'Pronto para Retirada na Loja'),
    (4, 'Entregue'),
    (5, 'Saiu para Entrega'),
    (6, 'Cancelado');
    SET IDENTITY_INSERT Situacao OFF;
END
ELSE
BEGIN
    UPDATE Situacao SET descricao = 'Pronto para Retirada na Loja' WHERE idSituacao = 3;
    IF NOT EXISTS (SELECT 1 FROM Situacao WHERE idSituacao = 5)
    BEGIN
        SET IDENTITY_INSERT Situacao ON;
        INSERT INTO Situacao (idSituacao, descricao) VALUES (5, 'Saiu para Entrega');
        SET IDENTITY_INSERT Situacao OFF;
    END
    IF NOT EXISTS (SELECT 1 FROM Situacao WHERE idSituacao = 6)
    BEGIN
        SET IDENTITY_INSERT Situacao ON;
        INSERT INTO Situacao (idSituacao, descricao) VALUES (6, 'Cancelado');
        SET IDENTITY_INSERT Situacao OFF;
    END
END
GO

-- Produtos
IF NOT EXISTS (SELECT 1 FROM Produto)
BEGIN
    INSERT INTO Produto (nome, descricao, preco, disponivel, imagemUrl, idCategoria) VALUES
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

-- Clientes (Admin e Cliente de teste com senha '123456' convertida em SHA-256 pela Procedure)
IF NOT EXISTS (SELECT 1 FROM Cliente)
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
    -- Converte automaticamente qualquer senha antiga em texto puro para SHA-256
    UPDATE [dbo].[Cliente]
    SET [senhaAcesso] = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(255), [senhaAcesso])), 2)
    WHERE LEN([senhaAcesso]) <> 64;
END
GO

-- Promoções Ativas
IF NOT EXISTS (SELECT 1 FROM Promocao)
BEGIN
    INSERT INTO Promocao (titulo, descricao, percentualDesconto, dataInicio, dataFim) VALUES
    ('Semana do Chocolate Pediê', 'Aproveite desconto especial em todos os bolos e brownies de chocolate!', 15.00, DATEADD(day, -5, GETDATE()), DATEADD(day, 30, GETDATE())),
    ('Festival de Tortas Artesanais', 'Nossas tortas clássicas com desconto imperdível para adoçar seu dia.', 20.00, DATEADD(day, -2, GETDATE()), DATEADD(day, 15, GETDATE()));
END
GO
