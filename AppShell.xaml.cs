using CrmApp.Helpers;
using CrmApp.Views;

namespace CrmApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(Routes.ClientDetail, typeof(ClientDetailPage));
    }
}