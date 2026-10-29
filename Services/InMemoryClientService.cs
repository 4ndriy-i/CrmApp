using CrmApp.Models;

namespace CrmApp.Services;

public class InMemoryClientService : IClientService
{
    private readonly List<Client> _clients = new()
    {
        new Client { Name = "Олена Коваль", Company = "ФОП Коваль", Phone = "+380501112233", DealAmount = 15000m },
        new Client { Name = "Андрій Мельник", Company = "ТОВ «Альфа»", Phone = "+380672223344", DealAmount = 42500m }
    };

    public IReadOnlyList<Client> GetAll() => _clients.ToList();
    public void Add(Client client) => _clients.Add(client);
    public void Remove(Client client) => _clients.Remove(client);
}