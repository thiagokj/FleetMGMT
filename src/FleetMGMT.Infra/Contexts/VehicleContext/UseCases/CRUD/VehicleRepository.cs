using System.Data;
using Dapper;
using FleetMGMT.Core.Contexts.VehicleContext.Entities;
using FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
using FleetMGMT.Infra.Contexts.DataContext;

namespace FleetMGMT.Infra.Contexts.VehicleContext.UseCases.CRUD;
public class VehicleRepository(AppDbContext context) : IRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Vehicle?>> GetAllAsync()
    {
        return await _context
            .Connection
            .QueryAsync<Vehicle>(
            "spGetAllVehicles", null,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        return await _context
            .Connection
            .QuerySingleOrDefaultAsync<Vehicle>(
                "spGetVehicleById", parameters,
                commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(Vehicle model)
    {
        return await _context
        .Connection
        .ExecuteAsync("spCreateVehicle", new
        {
            model.Name,
            model.Brand,
            model.MarketPrice,
            model.LeaseStart,
            model.LeaseEnd,
            model.Renter
        });
    }

    public async Task<int> UpdateAsync(Guid id, Vehicle model)
    {
        var sql = @$"
            UPDATE [Vehicle]
            SET
                [Name] = @Name,
                [Brand] = @Brand,
                [MarketPrice] = @MarketPrice,
                [LeaseStart] = @LeaseStart,
                [LeaseEnd] = @LeaseEnd,
                [Renter] = @Renter
            WHERE
                [Id] = @id";

        return await _context
        .Connection
        .ExecuteAsync(sql, new
        {
            Id = id,
            model.Name,
            model.Brand,
            model.MarketPrice,
            model.LeaseStart,
            model.LeaseEnd,
            model.Renter
        });
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM [Vehicle] WHERE [Id] = @id";
        return await _context.Connection.ExecuteAsync(sql, new { Id = id });
    }
}