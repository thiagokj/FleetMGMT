using Microsoft.AspNetCore.Mvc;
using FleetMGMT.UI.Contexts.VehicleContext.Models;
using FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD;

namespace FleetMGMT.UI.Contexts.VehicleContext.Controllers
{
    public class VehiclesController(VehicleService vehicleService) : Controller
    {
        private readonly VehicleService _vehicleService = vehicleService;

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return View(vehicles);
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create() => View(new VehicleViewModel());

        // POST: Vehicles/Create
        [HttpPost]
        public async Task<IActionResult> Create(VehicleViewModel vehicle)
        {
            if (!ModelState.IsValid) return View(vehicle);

            try
            {
                await _vehicleService.CreateVehicleAsync(vehicle);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, VehicleViewModel vehicle)
        {
            try
            {
                await _vehicleService.UpdateVehicleAsync(id, vehicle);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // DELETE: Vehicles/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _vehicleService.DeleteVehicleAsync(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
