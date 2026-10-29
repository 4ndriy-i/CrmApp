namespace CrmApp.Models;

public class Client
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal DealAmount { get; set; }
}