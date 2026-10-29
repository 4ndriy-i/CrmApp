using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrmApp.Helpers;
using CrmApp.Models;
using CrmApp.Services;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CrmApp.ViewModels;

public partial class ClientDetailViewModel : ObservableObject, IQueryAttributable, ILifecycleAware
{
    private readonly IClientService _clientService;
    private readonly INavigationService _navigation;
    private Client? _client;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string name = string.Empty;

    [ObservableProperty] private string company = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string dealAmountText = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ClientDetailViewModel(IClientService clientService, INavigationService navigation)
    {
        _clientService = clientService;
        _navigation = navigation;
    }

    // Отримання параметра навігації (?id=...)
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var value) || !Guid.TryParse(value?.ToString(), out var id))
            return;

        _client = _clientService.GetById(id);
        if (_client is null)
        {
            StatusMessage = "Клієнта не знайдено.";
            return;
        }

        Name = _client.Name;
        Company = _client.Company;
        Phone = _client.Phone;
        DealAmountText = _client.DealAmount.ToString("0.##", CultureInfo.InvariantCulture);

        // Стан застосунку: останній переглянутий клієнт
        Preferences.Default.Set(PreferenceKeys.LastClientId, id.ToString());
    }

    public void OnAppearing() => Debug.WriteLine("[ClientDetailViewModel] OnAppearing");
    public void OnDisappearing() => Debug.WriteLine("[ClientDetailViewModel] OnDisappearing");

    private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (_client is null) return;

        if (!string.IsNullOrWhiteSpace(DealAmountText)
            && !InputParser.TryParseAmount(DealAmountText, out _))
        {
            StatusMessage = "Некоректна сума угоди.";
            return;
        }
        InputParser.TryParseAmount(DealAmountText, out var amount);

        _clientService.Update(new Client
        {
            Id = _client.Id,
            Name = Name.Trim(),
            Company = Company.Trim(),
            Phone = Phone.Trim(),
            DealAmount = amount
        });

        await _navigation.GoBackAsync();
    }

    [RelayCommand]
    private Task CancelAsync() => _navigation.GoBackAsync();
}