using HealthSync.Data;
using HealthSync.Models;

namespace HealthSync.Views;

public partial class DashboardPage : ContentPage
{
	private DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }

    public DashboardPage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
        IngelogdeUser = ingelogdeUser;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = null;
        BindingContext = this;
    }

    private async void Noodhulp_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Noodhulp", "Nog niet geïmplementeerd", "OK");
    }
}