using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Services;

namespace HealthSync.Views;

public partial class CountersPage : ContentPage
{
    private readonly DatabaseOperaties _database;
    private readonly User _user;

    public CountersPage(DatabaseOperaties database, User user)
    {
        InitializeComponent();
        _database = database;
        _user = user;
    }

    private async void BtnResetSteps_Clicked(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert("Reset steps", "Reset step counter?", "Yes", "No");
        if (ok)
        {
            CounterResetService.RequestResetSteps();
            await DisplayAlert("Done", "Step counter reset.", "OK");
        }
    }

    private async void BtnResetKcal_Clicked(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert("Reset kcal", "Reset kcal counter?", "Yes", "No");
        if (ok)
        {
            CounterResetService.RequestResetKcal();
            await DisplayAlert("Done", "Kcal counter reset.", "OK");
        }
    }

    private async void BtnResetSleep_Clicked(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert("Reset sleep", "Reset sleep counter?", "Yes", "No");
        if (ok)
        {
            CounterResetService.RequestResetSleep();
            await DisplayAlert("Done", "Sleep counter reset.", "OK");
        }
    }
}