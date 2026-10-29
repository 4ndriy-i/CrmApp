using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrmApp.Helpers;
using CrmApp.Models;
using CrmApp.Services;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CrmApp.ViewModels;

public partial class ClientsViewModel : ObservableObject
{
    private readonly IClientService _clientService;

    public ObservableCollection<Client> Clients { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddClientCommand))]
    private string name = string.Empty;

    [ObservableProperty] private string company = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string dealAmountText = string.Empty;
    [ObservableProperty] private decimal totalDeals;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ClientsViewModel(IClientService clientService)
    {
        _clientService = clientService;
        foreach (var c in _clientService.GetAll())
            Clients.Add(c);
        RecalculateTotal();
    }

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

    private void RecalculateTotal() => TotalDeals = Clients.Sum(c => c.DealAmount);
}