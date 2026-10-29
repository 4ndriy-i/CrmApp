using System.Diagnostics;
using CrmApp.ViewModels;

namespace CrmApp;

public partial class App : Application
{
    private readonly ClientsViewModel _clientsViewModel;

    public App(ClientsViewModel clientsViewModel)
    {
        InitializeComponent();
        _clientsViewModel = clientsViewModel;
        Debug.WriteLine("[App] Constructor");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Debug.WriteLine("[App] CreateWindow");
        var window = new Window(new AppShell());

        window.Created += (_, _) => Debug.WriteLine("[Window] Created");
        window.Activated += (_, _) => Debug.WriteLine("[Window] Activated");
        window.Deactivated += (_, _) => Debug.WriteLine("[Window] Deactivated");
        window.Stopped += (_, _) => Debug.WriteLine("[Window] Stopped");
        window.Resumed += (_, _) => Debug.WriteLine("[Window] Resumed");
        window.Destroying += (_, _) =>
        {
            Debug.WriteLine("[Window] Destroying");
            _clientsViewModel.SaveState();
        };

        return window;
    }

    protected override void OnStart() => Debug.WriteLine("[App] OnStart");

    protected override void OnSleep()
    {
        Debug.WriteLine("[App] OnSleep");
        _clientsViewModel.SaveState();
    }

    protected override void OnResume() => Debug.WriteLine("[App] OnResume");
}