namespace FleetMGMT.UI.Contexts.VehicleContext.Models;
public class VehicleViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public decimal MarketPrice { get; set; }
    public DateTime LeaseStart { get; set; }
    public DateTime LeaseEnd { get; set; }
    public string Renter { get; set; } = null!;
}
