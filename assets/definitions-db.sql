-- Cria o Banco
CREATE DATABASE FleetMGMTDB
COLLATE SQL_Latin1_General_CP1_CI_AI;

-- Cria a tabela de Veículos
USE FleetMGMTDB;
CREATE TABLE Vehicle
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(150) NOT NULL,
    [Brand] NVARCHAR(150),
    [MarketPrice] DECIMAL(18, 2),
    [LeaseStart] DATETIME NOT NULL,
    [LeaseEnd] DATETIME NOT NULL,
    [Renter] NVARCHAR(255) NOT NULL
);

-- Cria a procedure para retornar todos os veículos
CREATE PROCEDURE spGetAllVehicles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
    [Id],
    [Name],
    [Brand],
    [MarketPrice],
    [LeaseStart],
    [LeaseEnd],
    [Renter]
    FROM Vehicle;
END;

-- Cria procedure para retornar o veículo por Id
CREATE PROCEDURE spGetVehicleById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
    [Id],
    [Name],
    [Brand],
    [MarketPrice],
    [LeaseStart],
    [LeaseEnd],
    [Renter]
    FROM Vehicle
    WHERE Id = @Id;
END;

-- Procedure para inserir um veiculo
CREATE PROCEDURE spCreateVehicle
    @Name NVARCHAR(150),
    @Brand NVARCHAR(150),
    @MarketPrice DECIMAL(18,2),
    @LeaseStart DATE,
    @LeaseEnd DATE,
    @Renter NVARCHAR(255)
AS
BEGIN
    INSERT INTO [Vehicle]
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
        NEWID(),
        @Name, 
        @Brand, 
        @MarketPrice, 
        @LeaseStart, 
        @LeaseEnd, 
        @Renter
    );
END
GO