using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrmApp.Helpers;
using CrmApp.Models;
using CrmApp.Services;
using Microsoft.Maui.Storage;

namespace CrmApp.ViewModels;

public partial class ClientsViewModel : ObservableObject, ILifecycleAware
{
    private readonly IClientRepository _repository;
    private readonly INavigationService _navigation;
    private bool _stateRestored;

    public ObservableCollection<Client> Clients { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddClientCommand))]
    private string name = string.Empty;

    [ObservableProperty] private string company = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string dealAmountText = string.Empty;
    [ObservableProperty] private decimal totalDeals;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string lastViewedText = string.Empty;
    [ObservableProperty] private Client? selectedClient;

    // Вибір елемента списку відкриває екран деталей
    partial void OnSelectedClientChanged(Client? value)
    {
        if (value is null) return;
        _ = NavigateToClientAsync(value);
    }

    public ClientsViewModel(IClientRepository repository, INavigationService navigation)
    {
        _repository = repository;
        _navigation = navigation;
    }

    // ---- Життєвий цикл ----
    public void OnAppearing()
    {
        Debug.WriteLine("[ClientsViewModel] OnAppearing");
        _ = LoadClientsAsync();   // асинхронне завантаження з бази

        if (!_stateRestored)
        {
            RestoreState();
            _stateRestored = true;
        }
    }

    public void OnDisappearing()
    {
        Debug.WriteLine("[ClientsViewModel] OnDisappearing");
        SaveState();
    }

    // ---- Робота з базою ----
    private async Task LoadClientsAsync()
    {
        try
        {
            var items = await _repository.GetAllAsync();
            Clients.Clear();
            foreach (var c in items)
                Clients.Add(c);

            RecalculateTotal();
            UpdateLastViewed();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ClientsViewModel] Load error: {ex}");
            StatusMessage = "Не вдалося завантажити дані.";
        }
    }

    // ---- Стан (Preferences) ----
    public void SaveState()
    {
        Preferences.Default.Set(PreferenceKeys.DraftName, Name);
        Preferences.Default.Set(PreferenceKeys.DraftCompany, Company);
        Preferences.Default.Set(PreferenceKeys.DraftPhone, Phone);
        Preferences.Default.Set(PreferenceKeys.DraftAmount, DealAmountText);
        Debug.WriteLine("[ClientsViewModel] State saved");
    }

    private void RestoreState()
    {
        Name = Preferences.Default.Get(PreferenceKeys.DraftName, string.Empty);
        Company = Preferences.Default.Get(PreferenceKeys.DraftCompany, string.Empty);
        Phone = Preferences.Default.Get(PreferenceKeys.DraftPhone, string.Empty);
        DealAmountText = Preferences.Default.Get(PreferenceKeys.DraftAmount, string.Empty);
        Debug.WriteLine("[ClientsViewModel] State restored");
    }

    // ---- Команди ----
    private bool CanAddClient() => !string.IsNullOrWhiteSpace(Name);

    [RelayCommand(CanExecute = nameof(CanAddClient))]
    private async Task AddClientAsync()
    {
        if (!string.IsNullOrWhiteSpace(DealAmountText)
            && !InputParser.TryParseAmount(DealAmountText, out _))
        {
            StatusMessage = "Некоректна сума угоди.";
            return;
        }

        InputParser.TryParseAmount(DealAmountText, out var amount);

        var client = new Client
        {
            Name = Name.Trim(),
            Company = Company.Trim(),
            Phone = Phone.Trim(),
            DealAmount = amount
        };

        try
        {
            await _repository.InsertAsync(client);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ClientsViewModel] Insert error: {ex}");
            StatusMessage = "Не вдалося зберегти клієнта.";
            return;
        }

        Clients.Add(client);
        RecalculateTotal();

        StatusMessage = $"Клієнта «{client.Name}» додано.";
        Name = Company = Phone = DealAmountText = string.Empty;
        SaveState();
    }

    [RelayCommand]
    private async Task DeleteClientAsync(Client? client)
    {
        if (client is null) return;

        try
        {
            await _repository.DeleteAsync(client);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ClientsViewModel] Delete error: {ex}");
            StatusMessage = "Не вдалося видалити клієнта.";
            return;
        }

        Clients.Remove(client);
        RecalculateTotal();
        StatusMessage = $"Клієнта «{client.Name}» видалено.";
    }

    private async Task NavigateToClientAsync(Client client)
    {
        SelectedClient = null; // скидаємо вибір, щоб можна було відкрити того самого клієнта ще раз
        await _navigation.GoToAsync($"{Routes.ClientDetail}?id={client.Id}");
    }

    // ---- Допоміжні методи ----
    private void UpdateLastViewed()
    {
        var id = Preferences.Default.Get(PreferenceKeys.LastClientId, string.Empty);
        var client = Clients.FirstOrDefault(c => c.Id.ToString() == id);
        LastViewedText = client is null ? string.Empty : $"Останній переглянутий: {client.Name}";
    }

    private void RecalculateTotal() => TotalDeals = Clients.Sum(c => c.DealAmount);
}