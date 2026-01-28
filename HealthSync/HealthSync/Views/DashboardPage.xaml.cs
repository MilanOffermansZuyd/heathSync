using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Models.Enums;
using HealthSync.Services;

namespace HealthSync.Views;

public partial class DashboardPage : ContentPage
{
	private readonly DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }

    public int ActiveMedicationCount { get; private set; }
    public int PendingRequestCount { get; private set; }
    public string LatestRequestTitleText { get; private set; } = "Nog geen aanvragen";
    public string LatestRequestStatusText { get; private set; } = "";
    public string LatestRequestTimeText { get; private set; } = "";

    public string LastRefreshedText { get; private set; } = "";

    private IDispatcherTimer? dashboardTimer;
    private bool dashboardRefreshRunning;
    private int refreshIntervalSeconds = 30;


    public DashboardPage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
        IngelogdeUser = ingelogdeUser;
        BindingContext = this;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = null;
        BindingContext = this;
        await LoadDashboardAsync();
        StartAutoRefresh();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopAutoRefresh();
    }

    private void StartAutoRefresh()
    {
        if (dashboardTimer != null) return;

        dashboardTimer = Dispatcher.CreateTimer();
        dashboardTimer.Interval = TimeSpan.FromSeconds(refreshIntervalSeconds);
        dashboardTimer.IsRepeating = true;

        dashboardTimer.Tick += async (s, e) =>
        {
            if (dashboardRefreshRunning) return;
            dashboardRefreshRunning = true;

            try
            {
                await LoadDashboardAsync();
            }
            finally
            {
                dashboardRefreshRunning = false;
            }
        };

        dashboardTimer.Start();
    }

    private void StopAutoRefresh()
    {
        if (dashboardTimer == null) return;

        dashboardTimer.Stop();
        dashboardTimer = null;
    }


    private async Task LoadDashboardAsync()
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
                // sync faalt -> laat lokale data zien
            }
        }

        var prescriptions = await Database.GetPrescriptionsByUserIdAsync(IngelogdeUser.Id);
        ActiveMedicationCount = prescriptions.Count;

        var requests = await Database.GetPrescriptionRequestsByUserIdAsync(IngelogdeUser.Id);
        PendingRequestCount = requests.Count(r => r.Status == RequestStatus.Pending);

        var latest = requests.OrderByDescending(r => r.DateOfRequest).FirstOrDefault();

        if (latest == null)
        {
            LatestRequestTitleText = "Nog geen aanvragen";
            LatestRequestStatusText = "";
            LatestRequestTimeText = "";
        }
        else
        {
            var medName = latest.Medication?.Name ?? $"Medicatie #{latest.MedicationId}";
            var doctorName = latest.Doctor?.FullName ?? $"Dokter #{latest.DoctorId}";

            LatestRequestTitleText = $"{medName} • {doctorName}";
            LatestRequestStatusText = latest.Status.ToString();

            if (latest.Status == RequestStatus.Pending)
            {
                LatestRequestTimeText = $"Aangevraagd: {latest.DateOfRequest:dd-MM-yyyy HH:mm}";
            }
            else if (latest.Status == RequestStatus.Approved)
            {
                LatestRequestTimeText = latest.DateOfResponse.HasValue
                    ? $"Goedgekeurd: {latest.DateOfResponse.Value:dd-MM-yyyy HH:mm}"
                    : "Goedgekeurd: (datum onbekend)";
            }
            else if (latest.Status == RequestStatus.Denied)
            {
                LatestRequestTimeText = latest.DateOfResponse.HasValue
                    ? $"Afgekeurd: {latest.DateOfResponse.Value:dd-MM-yyyy HH:mm}"
                    : "Afgekeurd: (datum onbekend)";
            }
            else
            {
                // fallback voor eventuele nieuwe status
                LatestRequestTimeText = $"Aangevraagd: {latest.DateOfRequest:dd-MM-yyyy HH:mm}";
            }
        }


        LastRefreshedText = DateTime.Now.ToString("dd-MM-yyyy HH:mm");

        OnPropertyChanged(nameof(ActiveMedicationCount));
        OnPropertyChanged(nameof(PendingRequestCount));
        OnPropertyChanged(nameof(LatestRequestTitleText));
        OnPropertyChanged(nameof(LatestRequestStatusText));
        OnPropertyChanged(nameof(LatestRequestTimeText));
        OnPropertyChanged(nameof(LastRefreshedText));
    }



    private async void Noodhulp_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CallEmergencyContactPage(Database, IngelogdeUser));
    }
}