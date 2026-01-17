using HealthSync.Data;
using HealthSync.Services;
using HealthSync.Views.Auth;

namespace HealthSync.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseOperaties Database;

	public LoginPage(DatabaseOperaties database)
	{
		InitializeComponent();
        Database = database;
	}

    
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Login.IsEnabled = true;
        Registreren.IsEnabled = true;

        // Internetcheck bij opstarten
        if (!Constanten.IsInternetAvailable())
        {
            await DisplayAlert( "Geen internet", "Controleer de internetverbinding!", "OK" );
            Login.IsEnabled = false;
            Registreren.IsEnabled = false;
            return;
        }

        // HIERMEE VINDT JE HET PAD NAAR DE DATABASE
        // await DisplayAlert("DB pad", Constanten.GetDatabaseFilePath(), "OK");
    }


    private async void Login_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Login", "Nog niet geimplementeerd, verwijst nu alleen door", "OK");

        await Navigation.PushAsync(new MainTabbedPage
            (new DashboardPage(Database),
            new LifestylePage(Database), 
            new MedicatiePage(Database)));
    }

    private async void Registreren_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(Database));
    }
}