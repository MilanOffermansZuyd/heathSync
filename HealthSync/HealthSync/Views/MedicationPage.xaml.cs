using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Services;
using System.Collections.ObjectModel;

namespace HealthSync.Views;

public partial class MedicationPage : ContentPage
{
	private readonly DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }
    public ObservableCollection<Prescription> Prescriptions { get; } = new();
    public ObservableCollection<PrescriptionRequest> PrescriptionRequests { get; } = new();

    private IDispatcherTimer? syncTimer;
    private bool syncRunning;
    private int syncIntervalSeconds = 30;

    public MedicationPage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
		IngelogdeUser = ingelogdeUser;
		BindingContext = this;
        SetTab(false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SyncAndRefreshAsync();
        StartAutoSync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopAutoSync();
    }

    private void StartAutoSync()
    {
        if (syncTimer != null) return;

        syncTimer = Dispatcher.CreateTimer();
        syncTimer.Interval = TimeSpan.FromSeconds(syncIntervalSeconds);
        syncTimer.IsRepeating = true;

        syncTimer.Tick += async (s, e) =>
        {
            if (syncRunning) return;
            syncRunning = true;

            try
            {
                await SyncAndRefreshAsync();
            }
            finally
            {
                syncRunning = false;
            }
        };

        syncTimer.Start();
    }

    private void StopAutoSync()
    {
        if (syncTimer == null) return;

        syncTimer.Stop();
        syncTimer = null;
    }

    private async Task RefreshFromLocalAsync()
    {
        Prescriptions.Clear();
        var prescriptions = await Database.GetPrescriptionsByUserIdAsync(IngelogdeUser.Id);
        foreach (var p in prescriptions)
            Prescriptions.Add(p);

        PrescriptionRequests.Clear();
        var requests = await Database.GetPrescriptionRequestsByUserIdAsync(IngelogdeUser.Id);
        foreach (var r in requests)
            PrescriptionRequests.Add(r);
    }

    private async Task SyncAndRefreshAsync()
    {
        if (Constanten.IsInternetAvailable())
        {
            try
            {
                var sp = Application.Current?.Handler?.MauiContext?.Services;
                var api = (ApiService?)sp?.GetService(typeof(ApiService));

                if (api != null)
                    await Database.SyncPrescriptionDataFromApiAsync(api, IngelogdeUser.Id);
            }
            catch
            {
                // sync faalt -> gewoon lokaal tonen
            }
        }

        await RefreshFromLocalAsync();
    }


    private void SetTab(bool toonAanvragen)
    {
        MedicatieView.IsVisible = !toonAanvragen;
        AanvragenView.IsVisible = toonAanvragen;

        MedicationRequest.IsVisible = !toonAanvragen;

        // Visueel: actief = 1.0, inactief = 0.65
        BtnTabMedicatie.Opacity = toonAanvragen ? 0.65 : 1.0;
        BtnTabAanvragen.Opacity = toonAanvragen ? 1.0 : 0.65;

        // Visueel: actief = Bold
        BtnTabMedicatie.FontAttributes = toonAanvragen ? FontAttributes.None : FontAttributes.Bold;
        BtnTabAanvragen.FontAttributes = toonAanvragen ? FontAttributes.Bold : FontAttributes.None;
    }


    private async void TabMedicatie_Clicked(object sender, EventArgs e)
    {
        await SyncAndRefreshAsync();
        SetTab(false);
    }

    private async void TabAanvragen_Clicked(object sender, EventArgs e)
    {
        await SyncAndRefreshAsync();
        SetTab(true);
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