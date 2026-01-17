using HealthSync.Models.Enums;

namespace HealthSync.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string VolledigeNaam => $"{Voornaam} {Achternaam}";
        public string Email { get; set; }
        public string Wachtwoord { get; set; }
        public UserRole Role { get; set; }

    }
}
