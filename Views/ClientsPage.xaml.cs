using CrmApp.ViewModels;

namespace CrmApp.Views;

public partial class ClientsPage : ContentPage
{
    public ClientsPage(ClientsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}