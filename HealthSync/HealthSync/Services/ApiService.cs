using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthSync.Models;

namespace HealthSync.Services;

public class ApiService
{
    private readonly HttpClient Http;

    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() } // enum strings lezen (Pending/Approved/Denied)
    };

    public ApiService(HttpClient http)
    {
        Http = http;
    }

    // POST: aanvraag naar API sturen
    public async Task<PrescriptionRequest> CreatePrescriptionRequestAsync(PrescriptionRequest localRequest)
    {
        if (localRequest.ClientRequestId == Guid.Empty)
            localRequest.ClientRequestId = Guid.NewGuid();

        // Alleen meesturen wat API nodig heeft
        var payload = new
        {
            clientRequestId = localRequest.ClientRequestId,
            userId = localRequest.UserId,
            medicationId = localRequest.MedicationId,
            doctorId = localRequest.DoctorId,
            pharmacyId = localRequest.PharmacyId,
            note = localRequest.Note
        };

        using var response = await Http.PostAsJsonAsync("api/prescriptionrequests", payload, Json);
        response.EnsureSuccessStatusCode();

        var saved = await response.Content.ReadFromJsonAsync<PrescriptionRequest>(Json);
        return saved ?? throw new Exception("API gaf geen geldige response terug.");
    }

    // GET: aanvragen per user ophalen
    public async Task<List<PrescriptionRequest>> GetPrescriptionRequestsAsync(int userId)
    {
        var items = await Http.GetFromJsonAsync<List<PrescriptionRequest>>(
            $"api/prescriptionrequests?userId={userId}",
            Json);

        return items ?? new List<PrescriptionRequest>();
    }

    // Alleen voor debug (jouw /all endpoint)
    public async Task<List<PrescriptionRequest>> GetAllPrescriptionRequestsAsync()
    {
        var items = await Http.GetFromJsonAsync<List<PrescriptionRequest>>(
            "api/prescriptionrequests/all",
            Json);

        return items ?? new List<PrescriptionRequest>();
    }
}
