using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrmApp.Helpers;
using CrmApp.Models;
using CrmApp.Services;
using Microsoft.Maui.Storage;

namespace CrmApp.ViewModels;

public partial class ClientDetailViewModel : ObservableObject, IQueryAttributable, ILifecycleAware
{
    private readonly IClientRepository _repository;
    private readonly INavigationService _navigation;
    private Client? _client;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string name = string.Empty;

    [ObservableProperty] private string company = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string dealAmountText = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ClientDetailViewModel(IClientRepository repository, INavigationService navigation)
    {
        _repository = repository;
        _navigation = navigation;
    }

    // Отримання параметра навігації (?id=...)
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var value) || !int.TryParse(value?.ToString(), out var id))
            return;

        _ = LoadClientAsync(id);
    }

    private async Task LoadClientAsync(int id)
    {
        try
        {
            _client = await _repository.GetByIdAsync(id);
            if (_client is null)
            {
                StatusMessage = "Клієнта не знайдено.";
                return;
            }

            Name = _client.Name;
            Company = _client.Company;
            Phone = _client.Phone;
            DealAmountText = _client.DealAmount.ToString("0.##", CultureInfo.InvariantCulture);

            Preferences.Default.Set(PreferenceKeys.LastClientId, id.ToString());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ClientDetailViewModel] Load error: {ex}");
            StatusMessage = "Не вдалося завантажити клієнта.";
        }
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

        _client.Name = Name.Trim();
        _client.Company = Company.Trim();
        _client.Phone = Phone.Trim();
        _client.DealAmount = amount;

        try
        {
            await _repository.UpdateAsync(_client);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ClientDetailViewModel] Update error: {ex}");
            StatusMessage = "Не вдалося зберегти зміни.";
            return;
        }

        await _navigation.GoBackAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (_client is null) return;

        try
        {
            await _repository.DeleteAsync(_client);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ClientDetailViewModel] Delete error: {ex}");
            StatusMessage = "Не вдалося видалити клієнта.";
            return;
        }

        await _navigation.GoBackAsync();
    }

    [RelayCommand]
    private Task CancelAsync() => _navigation.GoBackAsync();
}