namespace HealthSync.Views;

public partial class MainTabbedPage : TabbedPage
{
    public MainTabbedPage(DashboardPage dashboardPage, LifestylePage lifestylePage, MedicationPage medicationPage, MenuPage menuPage)
    {
        InitializeComponent();
        Children.Add(new NavigationPage(dashboardPage) { Title = "Dashboard" });
        Children.Add(new NavigationPage(lifestylePage) { Title = "Lifestyle" });
        Children.Add(new NavigationPage(medicationPage) { Title = "Medication" });
        Children.Add(new NavigationPage(menuPage) { Title = "Menu" });
    }
}