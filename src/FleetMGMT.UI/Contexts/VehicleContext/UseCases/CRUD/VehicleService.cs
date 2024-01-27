using System.Text;
using System.Text.Json;
using FleetMGMT.UI.Contexts.VehicleContext.Models;
using FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD.Contracts;

namespace FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD;
public class VehicleService(HttpClient httpClient) : IVehicleService
{
    private readonly HttpClient _httpClient = httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IEnumerable<VehicleViewModel>> GetAllVehiclesAsync()
    {
        var response = await _httpClient.GetAsync("/v1/api/vehicles");
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();

        var vehicles = await JsonSerializer
            .DeserializeAsync<IEnumerable<VehicleViewModel>>(stream, _jsonOptions);

        return vehicles ?? [];
    }

    public async Task<VehicleViewModel> GetVehicleByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/v1/api/vehicles/{id}");
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();

        var vehicle = await JsonSerializer
            .DeserializeAsync<VehicleViewModel>(stream, _jsonOptions)
            ?? throw new Exception("Vehicle not found");

        return vehicle;
    }


    public async Task CreateVehicleAsync(VehicleViewModel vehicle)
    {
        var content = new StringContent(JsonSerializer.Serialize(vehicle), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/v1/api/vehicles", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateVehicleAsync(Guid id, VehicleViewModel vehicle)
    {
        var content = new StringContent(JsonSerializer.Serialize(vehicle), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/v1/api/vehicles/{id}", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteVehicleAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/v1/api/vehicles/{id}");
        response.EnsureSuccessStatusCode();
    }

}