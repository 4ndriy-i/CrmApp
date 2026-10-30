using System.Diagnostics;
using CrmApp.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace CrmApp.Services;

public class DatabaseService : IClientRepository
{
    private const string DbFileName = "crm.db3";
    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    private SQLiteAsyncConnection? _db;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    // Ліниве створення з'єднання й таблиці (один раз за час роботи)
    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db is not null) return _db;

        await _initLock.WaitAsync();
        try
        {
            if (_db is null)
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, DbFileName);
                Debug.WriteLine($"[DatabaseService] DB path: {path}");

                var db = new SQLiteAsyncConnection(path, Flags);
                await db.CreateTableAsync<Client>(); // схема з атрибутів моделі
                _db = db;
            }
            return _db;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<List<Client>> GetAllAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<Client>().OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        var db = await GetConnectionAsync();
        return await db.Table<Client>().Where(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> InsertAsync(Client client)
    {
        var db = await GetConnectionAsync();
        return await db.InsertAsync(client); // після вставки client.Id заповнюється
    }

    public async Task<int> UpdateAsync(Client client)
    {
        var db = await GetConnectionAsync();
        return await db.UpdateAsync(client);
    }

    public async Task<int> DeleteAsync(Client client)
    {
        var db = await GetConnectionAsync();
        return await db.DeleteAsync(client);
    }
}
