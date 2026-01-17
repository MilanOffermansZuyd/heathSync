namespace HealthSync.Views;

public partial class MainTabbedPage : TabbedPage
{
    public MainTabbedPage(
        DashboardPage dashboardPage,
        LifestylePage lifestylePage,
        MedicatiePage medicatiePage)
    {
        Title = "HealthSync";

        // Classic navigation per tab
        Children.Add(new NavigationPage(dashboardPage) { Title = "Dashboard" });
        Children.Add(new NavigationPage(lifestylePage) { Title = "Lifestyle" });
        Children.Add(new NavigationPage(medicatiePage) { Title = "Medicatie" });
    }
}