using FleetMGMT.Core.Contexts.VehicleContext.Entities;
using FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FleetMGMT.Api.Controllers;

public class VehicleController(IRepository repository) : ControllerBase
{
    private readonly IRepository _repository = repository;

    /// <summary>
    /// Obtém lista com todos os veículos.
    /// </summary>
    [HttpGet]
    [Route("v1/api/vehicles")]
    public async Task<IEnumerable<Vehicle?>> GetAllAsync() => await _repository.GetAllAsync();

    /// <summary>
    /// Obtém veículo por Id
    /// </summary>
    [HttpGet]
    [Route("v1/api/vehicles/{id}")]
    public async Task<Vehicle?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);

    /// <summary>
    /// Cria um novo veículo
    /// </summary>
    [HttpPost]
    [Route("v1/api/vehicles")]
    public async Task<IActionResult> CreateAsync([FromBody] Vehicle model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var affectedRows = await _repository.CreateAsync(model);

            return affectedRows > 0 ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    /// <summary>
    /// Atualiza o veículo com base no Id
    /// </summary>
    [HttpPut]
    [Route("v1/api/vehicles/{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] Vehicle model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var affectedRows = await _repository.UpdateAsync(id, model);

            return affectedRows > 0 ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    /// <summary>
    /// Exclui o veículo com base no Id
    /// </summary>
    [HttpDelete]
    [Route("v1/api/vehicles/{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        try
        {
            var affectedRows = await _repository.DeleteAsync(id);

            return affectedRows > 0 ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }
}