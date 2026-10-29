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
    private readonly IClientService _clientService;
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

    public ClientsViewModel(IClientService clientService, INavigationService navigation)
    {
        _clientService = clientService;
        _navigation = navigation;
        RefreshClients();
    }

    // ---- Життєвий цикл ----
    public void OnAppearing()
    {
        Debug.WriteLine("[ClientsViewModel] OnAppearing");
        RefreshClients(); // підхоплює зміни з екрана деталей

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

    // ---- Збереження / відновлення стану (Preferences) ----
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
    private void AddClient()
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

        _clientService.Add(client);
        Clients.Add(client);
        RecalculateTotal();

        StatusMessage = $"Клієнта «{client.Name}» додано.";
        Name = Company = Phone = DealAmountText = string.Empty;
        SaveState(); // чернетка очищена
    }

    [RelayCommand]
    private void DeleteClient(Client? client)
    {
        if (client is null) return;
        _clientService.Remove(client);
        Clients.Remove(client);
        RecalculateTotal();
        StatusMessage = $"Клієнта «{client.Name}» видалено.";
    }

    // Навігація на другий екран з параметром id
    [RelayCommand]
    private async Task OpenClientAsync(Client? client)
    {
        if (client is null) return;
        await _navigation.GoToAsync($"{Routes.ClientDetail}?id={client.Id}");
    }

    // ---- Допоміжні методи ----
    private void RefreshClients()
    {
        Clients.Clear();
        foreach (var c in _clientService.GetAll())
            Clients.Add(c);
        RecalculateTotal();
        UpdateLastViewed();
    }

    private void UpdateLastViewed()
    {
        var id = Preferences.Default.Get(PreferenceKeys.LastClientId, string.Empty);
        var client = Clients.FirstOrDefault(c => c.Id.ToString() == id);
        LastViewedText = client is null ? string.Empty : $"Останній переглянутий: {client.Name}";
    }

    private void RecalculateTotal() => TotalDeals = Clients.Sum(c => c.DealAmount);
}