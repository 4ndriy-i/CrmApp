using CrmApp.Models;

namespace CrmApp.Services;

public interface IClientService
{
    IReadOnlyList<Client> GetAll();
    void Add(Client client);
    void Remove(Client client);
}