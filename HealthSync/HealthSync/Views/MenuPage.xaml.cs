using HealthSync.Data;
using HealthSync.Models;

namespace HealthSync.Views;

public partial class MenuPage : ContentPage
{
	private readonly DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }
    public MenuPage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
        IngelogdeUser = ingelogdeUser;
        BindingContext = this;
    }

    private async void Uitloggen_Clicked(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert("Uitloggen", "Weet je het zeker?", "Ja", "Nee");
        if (!ok) return;

        this.Window.Page = new NavigationPage(new LoginPage(Database));
    }


    private async void ProfielInstellingen_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage(Database, IngelogdeUser));
    }

    private async void Noodhulp_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new EmergencyHelpPage(Database, IngelogdeUser));
    }
}