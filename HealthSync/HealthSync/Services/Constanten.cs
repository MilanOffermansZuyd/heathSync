namespace HealthSync.Services
{
    public class Constanten
    {
        // Internet beschikbaarheid controleren
        public static bool IsInternetAvailable()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }

        //^Validatie
        //        if (!LocationService.IsInternetAvailable())
        //{
        //    await DisplayAlert(
        //        "Geen internet",
        //        "Controleer de internetverbinding!",
        //        "OK"
        //    );
        //    return;
        //}

        public static string GetDatabaseFilePath()
        {
            var folderPath = FileSystem.AppDataDirectory;
            return Path.Combine(folderPath, "Vrienden.db");
        }
    }
}
