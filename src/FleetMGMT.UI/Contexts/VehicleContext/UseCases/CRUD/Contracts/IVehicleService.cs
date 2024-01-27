using FleetMGMT.UI.Contexts.VehicleContext.Models;

namespace FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD.Contracts;
public interface IVehicleService
{
    Task<IEnumerable<VehicleViewModel>> GetAllVehiclesAsync();
    Task<VehicleViewModel> GetVehicleByIdAsync(Guid id);
    Task CreateVehicleAsync(VehicleViewModel vehicle);
    Task UpdateVehicleAsync(Guid id, VehicleViewModel vehicle);
    Task DeleteVehicleAsync(Guid id);
}