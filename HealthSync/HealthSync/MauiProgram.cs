using HealthSync.Data;
using HealthSync.Services;
using HealthSync.Views;
using HealthSync.Views.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace HealthSync
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            string SQLiteDbPath = Constanten.GetDatabaseFilePath();
            builder.Services.AddDbContext<HealthSyncContext>(
                opt => opt.UseSqlite($"Data Source={SQLiteDbPath}")
            );


            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseOperaties>();

            builder.Services.AddTransient<LoginPage>();

            // KAN WSS WEG
            //builder.Services.AddTransient<RegisterPage>();
            //builder.Services.AddTransient<DashboardPage>();
            //builder.Services.AddTransient<MedicationPage>();
            //builder.Services.AddTransient<LifestylePage>();

            //builder.Services.AddTransient<MainTabbedPage>();
            string baseUrl =
                #if ANDROID
                    "https://10.0.2.2:7112/";      // Android emulator -> jouw PC localhost
                #else
                    "https://localhost:7112/";      // Windows (en meestal ook Mac)
                #endif

            // 2) HttpClient registreren (met dev-cert bypass in DEBUG)
            builder.Services.AddSingleton(sp =>
            {
                var handler = new HttpClientHandler();

#if DEBUG
                // Alleen voor development! (handig bij self-signed HTTPS op Android/Windows)
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(baseUrl)
                };
            });

            // 3) ApiService registreren
            builder.Services.AddSingleton<ApiService>();

            return builder.Build();
        }
    }
}
