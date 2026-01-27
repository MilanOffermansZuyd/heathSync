using System;

namespace HealthSync.Services
{
    public static class CounterResetService
    {
        public static event Action? ResetStepsRequested;
        public static event Action? ResetKcalRequested;
        public static event Action? ResetSleepRequested;

        public static void RequestResetSteps() => ResetStepsRequested?.Invoke();
        public static void RequestResetKcal() => ResetKcalRequested?.Invoke();
        public static void RequestResetSleep() => ResetSleepRequested?.Invoke();
    }
}