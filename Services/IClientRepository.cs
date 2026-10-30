using CrmApp.Models;

namespace CrmApp.Services;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
    Task<int> InsertAsync(Client client);
    Task<int> UpdateAsync(Client client);
    Task<int> DeleteAsync(Client client);
}