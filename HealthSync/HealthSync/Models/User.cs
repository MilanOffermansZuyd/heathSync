using HealthSync.Models.Enums;

namespace HealthSync.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public bool NotificationsEnabled { get; set; }
        public HealthData? HealthData { get; set; }
        public List<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>(); // Optioneel?
    }
}
