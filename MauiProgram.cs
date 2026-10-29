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

        builder.Services.AddSingleton<IClientService, InMemoryClientService>();
        builder.Services.AddSingleton<ClientsViewModel>();
        builder.Services.AddSingleton<ClientsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}