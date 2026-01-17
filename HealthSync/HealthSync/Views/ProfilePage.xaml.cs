using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Models.Enums;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace HealthSync.Views;

public partial class ProfilePage : ContentPage
{
    public ObservableCollection<UserRole> Roles { get; set; } = new ObservableCollection<UserRole>();
    public UserRole? GekozenRol { get; set; }

    private readonly DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }

	public ProfilePage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
        Database = database;
        IngelogdeUser = ingelogdeUser;
        BindingContext = this;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        EntVoornaam.Text = IngelogdeUser.Voornaam;
        EntAchternaam.Text = IngelogdeUser.Achternaam;
        EntEmail.Text = IngelogdeUser.Email;
        EntWachtwoord.Text = "";
        EntHerhaalWachtwoord.Text = "";

        if (Roles.Count == 0)
        {
            foreach (var role in Enum.GetValues(typeof(UserRole)).Cast<UserRole>())
                Roles.Add(role);
        }

        GekozenRol = IngelogdeUser.Role;
        PkrRole.SelectedItem = IngelogdeUser.Role;
    }

    private async void Opslaan_Clicked(object sender, EventArgs e)
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

        if (email != IngelogdeUser.Email)
        {
            bool emailBestaat = await Database.EmailBestaatAsync(email);
            if (emailBestaat)
            {
                await DisplayAlert("Fout", "Dit emailadres bestaat al.", "OK");
                return;
            }
        }

        if (GekozenRol == null)
        {
            await DisplayAlert("Fout", "Selecteer een rol", "OK");
            return;
        }

        IngelogdeUser.Role = GekozenRol.Value;


        if (!string.IsNullOrWhiteSpace(wachtwoord))
        {
            if (string.IsNullOrWhiteSpace(herhaalWachtwoord))
            {
                await DisplayAlert("Fout", "Vul het wachtwoord ook in bij 'Herhaal nieuw wachtwoord'.", "OK");
                return;
            }

            if (wachtwoord != herhaalWachtwoord)
            {
                await DisplayAlert("Fout", "Wachtwoorden komen niet overeen.", "OK");
                return;
            }

            if (wachtwoord.Length < 6)
            {
                await DisplayAlert("Fout", "Wachtwoord moet minimaal 6 tekens zijn.", "OK");
                return;
            }

            IngelogdeUser.Wachtwoord = wachtwoord;
        }

        if (string.IsNullOrWhiteSpace(wachtwoord))
        {
            if (!string.IsNullOrWhiteSpace(herhaalWachtwoord))
            {
                await DisplayAlert("Fout", "Vul ook het veld 'Nieuw wachtwoord' in.", "OK");
                return;
            }
        }

        IngelogdeUser.Voornaam = voornaam;
        IngelogdeUser.Achternaam = achternaam;
        IngelogdeUser.Email = email;

        await Database.UpdateUserAsync(IngelogdeUser);

        await DisplayAlert("Gelukt", "Gegevens opgeslagen", "OK");

        EntWachtwoord.Text = "";
        EntHerhaalWachtwoord.Text = "";

        await Navigation.PopAsync();
    }

    private async void Verwijderen_Clicked(object sender, EventArgs e)
    {
        bool verwijderenBevestigd = await DisplayAlert("Account verwijderen",
            "Weet je zeker dat je je account permanent wilt verwijderen?",
            "Ja",
            "Nee");

        if (!verwijderenBevestigd) return;

        await Database.DeleteUserAsync(IngelogdeUser);

        await DisplayAlert("Account verwijderd", "Je bent uitgelogd.", "OK");

        this.Window.Page = new NavigationPage(new LoginPage(Database));
    }
}