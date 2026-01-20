using HealthSync.Data;
using HealthSync.Models;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Collections.Generic;

namespace HealthSync.Views;

public partial class LifestylePage : ContentPage
{
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
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Hardcoded totdat smartwatch of stappen teller is toegevoegd
        try
        {
            SetSteps(50);
        }
        catch { }

        try
        {
            var sample = CreateSampleSleepSegments();
            PopulateHypnogram(sample);
        }
        catch { }
    }

    private void SetSteps(int steps)
    {
        currentSteps = steps;
        currentCalories = Math.Round(currentSteps * KcalPerStep, 1);

        try
        {
            var lblSteps = this.FindByName<Label>("LblSteps");
            var lblCalories = this.FindByName<Label>("LblCalories");

            if (lblSteps != null)
                lblSteps.Text = currentSteps.ToString();

            if (lblCalories != null)
                lblCalories.Text = $"{currentCalories} kcal";
        }
        catch { }
    }

    
    private void OnPedometerStepUpdate(int steps)
    {
        // Toekomst beeld voor toevoegen van smartwatch, stappen teller etc.
        SetSteps(steps);
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
            new SleepSegment("REM", 20),
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