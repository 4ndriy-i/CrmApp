using CrmApp.Models;

namespace CrmApp.Services;

public interface IClientService
{
    IReadOnlyList<Client> GetAll();
    Client? GetById(Guid id);
    void Add(Client client);
    void Update(Client client);
    void Remove(Client client);
}