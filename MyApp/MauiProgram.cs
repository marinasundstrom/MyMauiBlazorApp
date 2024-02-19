using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
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
