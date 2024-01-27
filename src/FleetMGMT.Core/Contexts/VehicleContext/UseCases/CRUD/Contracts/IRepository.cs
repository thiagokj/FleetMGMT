namespace FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
public interface IRepository
{
    Task<IEnumerable<Vehicle?>> GetAllAsync();
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<int> CreateAsync(Vehicle model);
    Task<int> UpdateAsync(Guid id, Vehicle model);
    Task<int> DeleteAsync(Guid id);
}