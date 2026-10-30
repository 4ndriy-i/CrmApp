using CrmApp.ViewModels;

namespace CrmApp.Views;

public partial class ClientDetailPage : ContentPage
{
    public ClientDetailPage(ClientDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ILifecycleAware)?.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        (BindingContext as ILifecycleAware)?.OnDisappearing();
    }
}