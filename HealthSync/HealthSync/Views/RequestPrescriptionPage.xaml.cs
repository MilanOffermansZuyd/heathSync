using HealthSync.Data;
using HealthSync.Models;
using HealthSync.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using HealthSync.Services;


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
            ClientRequestId = Guid.NewGuid(),
            UserId = IngelogdeUser.Id,
            MedicationId = gekozenMed.Id,
            DoctorId = gekozenDoctor.Id,
            PharmacyId = gekozenPharmacy.Id,
            Status = RequestStatus.Pending,
            DateOfRequest = DateTime.Now,
            Note = EdtNote.Text?.Trim()
        };

        await Database.AddPrescriptionRequestAsync(request);

        if (Constanten.IsInternetAvailable())
        {
            try
            {
                var sp = Application.Current?.Handler?.MauiContext?.Services;
                if (sp == null)
                    throw new Exception("ServiceProvider niet beschikbaar.");

                var api = sp.GetRequiredService<ApiService>();

                var savedFromApi = await api.CreatePrescriptionRequestAsync(request);

                request.RemoteId = savedFromApi.RemoteId;
                request.Status = savedFromApi.Status;
                request.DateOfRequest = savedFromApi.DateOfRequest;
                request.DateOfResponse = savedFromApi.DateOfResponse;

                request.ApprovedPrescriptionRemoteId = savedFromApi.ApprovedPrescriptionRemoteId;
                request.ApprovedPrescription = savedFromApi.ApprovedPrescription;


                await Database.UpdatePrescriptionRequestAsync(request);
            }
            catch
            {
                // API faalde -> lokaal staat hij al.
            }
        }

        await DisplayAlert("Aanvraag opgeslagen", "Je aanvraag is opgeslagen en staat op 'Pending'. Bekijk je status bij 'Mijn aanvragen'.", "OK");

        await Navigation.PopAsync();
    }
}