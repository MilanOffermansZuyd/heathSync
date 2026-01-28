using HealthSync.Data;
using HealthSync.Models;

namespace HealthSync.Views;

public partial class CallEmergencyContactPage : ContentPage
{
    private readonly DatabaseOperaties Database;
    private User IngelogdeUser;

    private int _countdown = 7;
    private System.Timers.Timer _timer;
    private bool _hasCalled = false;
    private CancellationTokenSource _countdownCts;

    public CallEmergencyContactPage(DatabaseOperaties database, User ingelogdeUser)
    {
        InitializeComponent();
        Database = database;
        IngelogdeUser = ingelogdeUser;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ResetState();
        await LoadContactsAsync();
        StartCountdown();
    }

    private void ResetState()
    {
        _timer?.Stop();
        _timer?.Dispose();

        _countdown = 7;
        _hasCalled = false;

        CountdownLabel.Text = $"Bellen over {_countdown}...";
    }

    private void StartCountdown()
    {
        _countdownCts = new CancellationTokenSource();

        _timer = new System.Timers.Timer(1000);
        _timer.AutoReset = true;
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (_countdownCts.IsCancellationRequested)
            return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_countdownCts.IsCancellationRequested)
                return;

            _countdown--;
            CountdownLabel.Text = $"Bellen over {_countdown}...";

            if (_countdown <= 0 && !_hasCalled)
            {
                _hasCalled = true;
                StopCountdown();
                CallNumber("112");
            }
        });
    }

    private void StopCountdown()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
    }

    private async Task LoadContactsAsync()
    {
        ContactsView.ItemsSource =
            await Database.GetEmergencyContactsByUserIdAsync(IngelogdeUser.Id);
    }

    private void OnContactSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is EmergencyContact contact)
        {
            _hasCalled = true;
            _timer.Stop();

            CallNumber(contact.PhoneNumber);

            ContactsView.SelectedItem = null;
        }
    }

    private async void CallNumber(string number)
    {
        try
        {
            if (PhoneDialer.IsSupported)
            {
                PhoneDialer.Open(number);
            }
            else
            {
                await DisplayAlert(
                    "Niet ondersteund",
                    "Bellen wordt niet ondersteund op dit apparaat / emulator.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", ex.Message, "OK");
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _countdownCts?.Cancel();
        _countdownCts?.Dispose();
        _countdownCts = null;

        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
    }
}