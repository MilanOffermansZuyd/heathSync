using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Services;
using System.Collections.ObjectModel;

namespace HealthSync.Views;

public partial class MedicationPage : ContentPage
{
	private DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }
    public ObservableCollection<Prescription> Prescriptions { get; } = new();
    public MedicationPage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
		IngelogdeUser = ingelogdeUser;
		BindingContext = this;	
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Prescriptions.Clear();
        var items = await Database.GetPrescriptionsByUserIdAsync(IngelogdeUser.Id);

        foreach (var p in items)
            Prescriptions.Add(p);
    }

    private async void MedicationRequest_Clicked(object sender, EventArgs e)
    {
        if (!Constanten.IsInternetAvailable())
        {
            await DisplayAlert("Geen internet", "Je hebt internet nodig om medicatie aan te vragen.", "OK");
            return;
        }

        await Navigation.PushAsync(new RequestPrescriptionPage(Database, IngelogdeUser));
    }
}