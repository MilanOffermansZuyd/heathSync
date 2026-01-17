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

        EntEmail.Text = "";
        EntWachtwoord.Text = "";

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
        string email = EntEmail.Text;
        string wachtwoord = EntWachtwoord.Text;

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Fout", "Vul een emailadres in", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(wachtwoord))
        {
            await DisplayAlert("Fout", "Vul een wachtwoord in", "OK");
            return;
        }

        email = email.Trim().ToLower();

        var user = await Database.GetUserByEmailAsync(email);

        if (user == null)
        {
            await DisplayAlert("Onbekend account", "Dit emailadres is niet geregistreerd.", "OK");
            return;
        }

        if (user.Wachtwoord != wachtwoord)
        {
            await DisplayAlert("Fout wachtwoord", "Het wachtwoord klopt niet.", "OK");
            return;
        }

        await Navigation.PushAsync(new MainTabbedPage(
            new DashboardPage(Database, user),
            new LifestylePage(Database, user),
            new MedicationPage(Database, user),
            new MenuPage(Database, user)
        ));

        Navigation.RemovePage(this);
    }

    private async void Registreren_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(Database));
    }
}