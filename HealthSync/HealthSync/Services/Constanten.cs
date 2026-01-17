namespace HealthSync.Services
{
    public class Constanten
    {
        public static bool IsInternetAvailable()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }

        public static string GetDatabaseFilePath()
        {
            var folderPath = FileSystem.AppDataDirectory;
            return Path.Combine(folderPath, "HealthSync.db");
        }
    }
}
