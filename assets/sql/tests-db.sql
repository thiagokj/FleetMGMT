USE FleetMGMTDB;

-- Insere 1registro de teste
INSERT INTO Vehicle
(
    [Id],
    [Name],
    [Brand],
    [MarketPrice],
    [LeaseStart],
    [LeaseEnd],
    [Renter]
)
VALUES
(
    NEWID(), -- Gera um novo valor UNIQUEIDENTIFIER
    'Carro ' + CAST((ABS(CHECKSUM(NEWID())) % 1000) AS VARCHAR), -- Gera um nome de carro aleatório
    'Marca ' + CAST((ABS(CHECKSUM(NEWID())) % 100) AS VARCHAR), -- Gera uma marca de carro aleatória
    10000 + (ABS(CHECKSUM(NEWID())) % 50000), -- Gera um preço de mercado aleatório
    DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 365), GETDATE()), -- Gera uma data de início de aluguel aleatória
    DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 365), GETDATE()), -- Gera uma data de término de aluguel aleatória
    'Locador ' + CAST((ABS(CHECKSUM(NEWID())) % 1000) AS VARCHAR) -- Gera um locador aleatório
);

-- Testes Procedures
-- Retorna todos os registros
DECLARE @RC int

EXECUTE @RC = [dbo].[spGetAllVehicles] 
GO

-- Executando teste
-- Retorna 1 registro
DECLARE @RC int
DECLARE @Id uniqueidentifier

EXECUTE @RC = [dbo].[spGetVehicleById] 
   @Id = "d13ae48d-7c96-48e8-9ae6-cfb8c70713a2";
GO

-- Executando teste
-- Insere 1 registro
DECLARE @RC int
DECLARE @Name nvarchar(150)
DECLARE @Brand nvarchar(150)
DECLARE @MarketPrice decimal(18,2)
DECLARE @LeaseStart date
DECLARE @LeaseEnd date
DECLARE @Renter nvarchar(255)

SET @Name = CONCAT('Carro ', CAST((RAND() * 1000) AS INT))
SET @Brand = CONCAT('Marca ', CAST((RAND() * 1000) AS INT))
SET @MarketPrice = CAST((RAND() * 100000) AS DECIMAL(18,2))
SET @LeaseStart = DATEADD(DAY, CAST((RAND() * 365) AS INT), GETDATE())
SET @LeaseEnd = DATEADD(DAY, CAST((RAND() * 365) AS INT), GETDATE())
SET @Renter = CONCAT('Locador ', CAST((RAND() * 1000) AS INT))

EXECUTE @RC = [dbo].[spCreateVehicle] 
  @Name,
  @Brand,
  @MarketPrice,
  @LeaseStart,
  @LeaseEnd,
  @Renter
GO