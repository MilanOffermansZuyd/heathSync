using HealthSync.Data;
using HealthSync.Models;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Collections.Generic;
using HealthSync.Services;
#if ANDROID
using Android.App;
using HealthSync.Platforms.Android;
using Microsoft.Maui.ApplicationModel;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Content.PM;
#endif


namespace HealthSync.Views;

public partial class LifestylePage : ContentPage
{
    #if ANDROID
        StepCounterService? _stepCounter;
    #endif
    private DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }

    // Steps and calories state
    private int currentSteps = 0;
    private double currentCalories = 0.0;

    // Simple conversion: average ~0.04 kcal per step (this depends on weight, speed; adjust later)
    private const double KcalPerStep = 0.04;

    public LifestylePage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
        IngelogdeUser = ingelogdeUser;
        BindingContext = this;

        // Subscribe to reset events
        CounterResetService.ResetStepsRequested += OnResetStepsRequested;
        CounterResetService.ResetKcalRequested += OnResetKcalRequested;
        CounterResetService.ResetSleepRequested += OnResetSleepRequested;

        #if ANDROID
                // Use fully-qualified Android application context to avoid ambiguity
                _stepCounter = new StepCounterService(Android.App.Application.Context);
                _stepCounter.StepsChanged += steps =>
                {
                    // Update using existing helper so UI and calories stay in sync
                    MainThread.BeginInvokeOnMainThread(() => SetSteps(steps));
                };
        #endif
    }

    private void OnResetStepsRequested()
    {
        // Reset UI steps to zero and instruct service to reset baseline
        MainThread.BeginInvokeOnMainThread(() => SetSteps(0));
        #if ANDROID
        StepCounterService.Instance?.ResetBaseline();
        #endif
    }

    private void OnResetKcalRequested()
    {
        // Reset calories only (keep steps)
        currentCalories = 0;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var lblCalories = this.FindByName<Label>("LblCalories");
            if (lblCalories != null) lblCalories.Text = "0 kcal";
        });
    }

    private void OnResetSleepRequested()
    {
        MainThread.BeginInvokeOnMainThread(() => ResetSleep());
    }

    private void ResetSleep()
    {
        // Clear hypnogram and reset total label
        var grid = this.FindByName<Grid>("HypnogramGrid");
        var lblTotal = this.FindByName<Label>("LblSleepTotal");
        if (grid != null) { grid.Children.Clear(); grid.ColumnDefinitions.Clear(); }
        if (lblTotal != null) lblTotal.Text = "0h 0m";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        #if ANDROID
                // Check and request ACTIVITY_RECOGNITION permission on Android (API 29+)
                try
                {
                    var context = Android.App.Application.Context;
                    if (ContextCompat.CheckSelfPermission(context, Android.Manifest.Permission.ActivityRecognition) != Permission.Granted)
                    {
                        var activity = Platform.CurrentActivity;
                        if (activity != null)
                        {
                            ActivityCompat.RequestPermissions(activity, new string[] { Android.Manifest.Permission.ActivityRecognition }, 1001);
                        }
                    }
                    else
                    {
                        _stepCounter?.Start();
                    }
                }
                catch { }

        #endif

        // Ensure UI starts at zero until sensor reports values
        try
        {
            SetSteps(0);
        }
        catch { }

        try
        {
            var sample = CreateSampleSleepSegments();
            PopulateHypnogram(sample);
        }
        catch { }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        #if ANDROID
                _stepCounter?.Stop();
        #endif
    }

    private void SetSteps(int steps)
    {
        currentSteps = steps;
        currentCalories = Math.Round(currentSteps * KcalPerStep, 1);

        try
        {
            // Correct name: StepsLabel (matches XAML)
            var lblSteps = this.FindByName<Label>("StepsLabel");
            var lblCalories = this.FindByName<Label>("LblCalories");

            if (lblSteps != null)
                lblSteps.Text = $"Stappen: {currentSteps}";

            if (lblCalories != null)
                lblCalories.Text = $"{currentCalories} kcal";
        }
        catch { }
    }

    private async void CounterSettings_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CountersPage(Database, IngelogdeUser));
    }

   

    // Slaap diagram 
    private record SleepSegment(string Stage, int Minutes);

    private List<SleepSegment> CreateSampleSleepSegments()
    {
        // Voorbeeld nacht
        return new List<SleepSegment>
        {
            new SleepSegment("Awake", 10),
            new SleepSegment("Light", 60),
            new SleepSegment("REM", 60),
            new SleepSegment("Light", 120),
            new SleepSegment("Deep", 40),
            new SleepSegment("REM", 30),
            new SleepSegment("Awake", 10)
        };
    }

    private void PopulateHypnogram(List<SleepSegment> segments)
    {
        var grid = this.FindByName<Grid>("HypnogramGrid");
        var lblTotal = this.FindByName<Label>("LblSleepTotal");
        if (grid == null) return;

        
        grid.ColumnDefinitions.Clear();
        grid.Children.Clear();

        // Color dict maken
        var colors = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            { "Awake", Color.FromArgb("#FFC107") },
            { "REM", Color.FromArgb("#FF0000") },
            { "Light", Color.FromArgb("#8BC34A") },
            { "Deep", Color.FromArgb("#800080") }
        };

        // kolommen toevoegen
        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(seg.Minutes, GridUnitType.Star) });

            var box = new BoxView
            {
                BackgroundColor = colors.ContainsKey(seg.Stage) ? colors[seg.Stage] : Color.FromArgb("#D3D3D3"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            Grid.SetColumn(box, i);
            Grid.SetRow(box, 0);
            grid.Children.Add(box);
        }

        // Slaap berekenen
        int totalMinutes = segments.Where(s => !string.Equals(s.Stage, "Awake", StringComparison.OrdinalIgnoreCase)).Sum(s => s.Minutes);
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        if (lblTotal != null)
            lblTotal.Text = $"{hours}h {minutes}m";
    }
}