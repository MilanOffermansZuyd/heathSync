using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Models.Enums;

namespace HealthSync.Views;

public partial class RequestPrescriptionPage : ContentPage
{
private readonly DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }

    public RequestPrescriptionPage(DatabaseOperaties database, User ingelogdeUser)
    {
        InitializeComponent();
        Database = database;
        IngelogdeUser = ingelogdeUser;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var medications = await Database.GetMedicationsAsync();
        PkrMedication.ItemsSource = medications;

        var doctors = await Database.GetDoctorsAsync();
        PkrDoctor.ItemsSource = doctors;

        var pharmacies = await Database.GetPharmaciesAsync();
        PkrPharmacy.ItemsSource = pharmacies;
    }

    private async void CancelRequest_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void RequestPrescription_Clicked(object sender, EventArgs e)
    {
        if (PkrMedication.SelectedItem == null)
        {
            await DisplayAlert("Ontbreekt", "Kies een medicatie.", "OK");
            return;
        }
        if (PkrDoctor.SelectedItem == null)
        {
            await DisplayAlert("Ontbreekt", "Kies een dokter.", "OK");
            return;
        }
        if (PkrPharmacy.SelectedItem == null)
        {
            await DisplayAlert("Ontbreekt", "Kies een apotheek.", "OK");
            return;
        }

        var gekozenMed = (Medication)PkrMedication.SelectedItem;
        var gekozenDoctor = (Doctor)PkrDoctor.SelectedItem;
        var gekozenPharmacy = (Pharmacy)PkrPharmacy.SelectedItem;


        var request = new PrescriptionRequest
        {
            UserId = IngelogdeUser.Id,
            MedicationId = gekozenMed.Id,
            DoctorId = gekozenDoctor.Id,
            PharmacyId = gekozenPharmacy.Id,

            Status = RequestStatus.Pending,
            DateOfRequest = DateTime.UtcNow,
            Note = EdtNote.Text?.Trim()
        };

        await Database.AddPrescriptionRequestAsync(request);

        await DisplayAlert("Aanvraag opgeslagen", "Je aanvraag is opgeslagen en staat op 'Pending'.", "OK");

        // Optioneel: velden resetten
        PkrMedication.SelectedItem = null;
        PkrDoctor.SelectedItem = null;
        PkrPharmacy.SelectedItem = null;
        EdtNote.Text = string.Empty;

        LblStatus.IsVisible = true;
        LblStatus.Text = "Aanvraag opgeslagen (Pending).";
    }
}