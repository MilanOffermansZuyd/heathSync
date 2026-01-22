namespace HealthSyncWebApi.Models
{
    public class ApprovePrescription
    {
        public int Amount { get; set; }
        public string Unit { get; set; } = "stuks";
        public int RefillsRemaining { get; set; }

        public int TakeCount { get; set; }
        public int TimeBetweenMinutes { get; set; }

        public string? Instruction { get; set; }
    }
}
