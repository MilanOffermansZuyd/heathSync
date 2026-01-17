using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HealthSync.Views.Auth;

public partial class RegisterPage : ContentPage
{
    public ObservableCollection<UserRole> Roles { get; set; } = new ObservableCollection<UserRole>();
    public UserRole? GekozenRol { get; set; }

    private readonly DatabaseOperaties Database;

    public RegisterPage(DatabaseOperaties database)
	{
		InitializeComponent();
        Database = database;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Roles.Count == 0)
        {
            foreach (var role in Enum.GetValues(typeof(UserRole)).Cast<UserRole>())
                Roles.Add(role);
        }
    }


    private async void AccountAanmaken_Clicked(object sender, EventArgs e)
    {
        string voornaam = EntVoornaam.Text;
        string achternaam = EntAchternaam.Text;
        string email = EntEmail.Text;
        string wachtwoord = EntWachtwoord.Text;
        string herhaalWachtwoord = EntHerhaalWachtwoord.Text;

        if (string.IsNullOrWhiteSpace(voornaam))
        {
            await DisplayAlert("Fout", "Vul een voornaam in", "OK");
            return;
        }

        voornaam = voornaam.Trim();

        if (string.IsNullOrWhiteSpace(achternaam))
        {
            await DisplayAlert("Fout", "Vul een achternaam in", "OK");
            return;
        }

        achternaam = achternaam.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Fout", "Vul een emailadres in", "OK");
            return;
        }

        email = email.Trim().ToLower();

        if (!email.Contains("@"))
        {
            await DisplayAlert("Fout", "Vul een geldig emailadres in", "OK");
            return;
        }

        if (GekozenRol == null)
        {
            await DisplayAlert("Fout", "Selecteer een rol", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(wachtwoord))
        {
            await DisplayAlert("Fout", "Vul een wachtwoord in", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(herhaalWachtwoord))
        {
            await DisplayAlert("Fout", "Vul ter controle het wachtwoord opnieuw in", "OK");
            return;
        }

        if (wachtwoord != herhaalWachtwoord)
        {
            await DisplayAlert("Fout", "Wachtwoorden komen niet overeen", "OK");
            return;
        }

        bool emailBestaat = await Database.EmailBestaatAsync(email);
        if (emailBestaat)
        {
            await DisplayAlert("Fout", "Dit emailadres bestaat al.", "OK");
            return;
        }

        User nieuweUser = new User
        {
            Voornaam = voornaam,
            Achternaam = achternaam,
            Email = email,
            Wachtwoord = wachtwoord,
            Role = GekozenRol.Value
        };

        await Database.AddUserAsync(nieuweUser);

        await DisplayAlert("Gelukt", "Account is aangemaakt!", "OK");

        await Navigation.PopAsync();
    }

    private async void TerugNaarLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}