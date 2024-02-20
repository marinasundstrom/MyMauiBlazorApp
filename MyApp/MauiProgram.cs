using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using ZXing.Net.Maui.Controls;
using MyApi;

namespace MyApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("SegoeUI-Regular.ttf", "Segoe UI");
            });

        builder.Services.AddFluentUIComponents();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddHttpClient("MyApi", httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://hlms1tb6-5286.uks1.devtunnels.ms");
        });

        builder.Services.AddHttpClient<IWeatherForecastClient>("MyApi")
                 .AddTypedClient<IWeatherForecastClient>((http, sp) => new WeatherForecastClient(http));

        return builder.Build();
    }
}
