using FleetMGMT.Core.Contexts.SharedContext.Entities;

namespace FleetMGMT.Core.Contexts.VehicleContext.Entities;
public class Vehicle : Entity
{
    public string Name { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public decimal MarketPrice { get; set; }
    public DateTime LeaseStart { get; set; }
    public DateTime LeaseEnd { get; set; }
    public string Renter { get; set; } = null!;
}