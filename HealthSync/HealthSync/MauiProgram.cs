using HealthSync.Data;
using HealthSync.Services;
using HealthSync.Views;
using HealthSync.Views.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            //builder.Services.AddTransient<MedicatiePage>();
            //builder.Services.AddTransient<LifestylePage>();

            //builder.Services.AddTransient<MainTabbedPage>();

            return builder.Build();
        }
    }
}
