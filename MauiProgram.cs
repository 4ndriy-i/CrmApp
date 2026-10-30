using CrmApp.Services;
using CrmApp.ViewModels;
using CrmApp.Views;
using Microsoft.Extensions.Logging;

namespace CrmApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Сервіси
        builder.Services.AddSingleton<IClientRepository, DatabaseService>();
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

        // Головний екран (один екземпляр)
        builder.Services.AddSingleton<ClientsViewModel>();
        builder.Services.AddSingleton<ClientsPage>();

        // Екран деталей (новий екземпляр при кожному переході)
        builder.Services.AddTransient<ClientDetailViewModel>();
        builder.Services.AddTransient<ClientDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}