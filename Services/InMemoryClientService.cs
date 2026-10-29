using CrmApp.Models;

namespace CrmApp.Services;

public class InMemoryClientService : IClientService
{
    private readonly List<Client> _clients = new()
    {
        new Client
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Олена Коваль", Company = "ФОП Коваль",
            Phone = "+380501112233", DealAmount = 15000m
        },
        new Client
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Андрій Мельник", Company = "ТОВ «Альфа»",
            Phone = "+380672223344", DealAmount = 42500m
        }
    };

    public IReadOnlyList<Client> GetAll() => _clients.ToList();
    public Client? GetById(Guid id) => _clients.FirstOrDefault(c => c.Id == id);
    public void Add(Client client) => _clients.Add(client);
    public void Remove(Client client) => _clients.Remove(client);

    public void Update(Client updated)
    {
        var existing = GetById(updated.Id);
        if (existing is null) return;
        existing.Name = updated.Name;
        existing.Company = updated.Company;
        existing.Phone = updated.Phone;
        existing.DealAmount = updated.DealAmount;
    }
}