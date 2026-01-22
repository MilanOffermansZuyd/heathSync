
using HealthSyncWebApi.Data;
using HealthSyncWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthSyncWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<ApiDbContext>(options =>
                options.UseSqlite("Data Source=APIHealthSync.db"));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                db.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/api/prescriptionrequests/all", async (ApiDbContext db) =>
            {
                var items = await db.PrescriptionRequests
                    .Include(r => r.ApprovedPrescription)
                    .OrderByDescending(r => r.DateOfRequest)
                    .ToListAsync();

                return Results.Ok(items);
            });


            // POST: aanvraag opslaan
            app.MapPost("/api/prescriptionrequests", async (ApiDbContext db, PrescriptionRequest input) =>
            {
                if (input.ClientRequestId == Guid.Empty)
                    return Results.BadRequest("ClientRequestId is verplicht.");

                // Dubbel posten voorkomen
                var bestaand = await db.PrescriptionRequests
                    .FirstOrDefaultAsync(r => r.ClientRequestId == input.ClientRequestId);

                if (bestaand != null)
                    return Results.Ok(bestaand);

                // Maak NIEUW object -> server bepaalt status + datums, client kan dat niet faken
                var request = new PrescriptionRequest
                {
                    ClientRequestId = input.ClientRequestId,
                    UserId = input.UserId,
                    MedicationId = input.MedicationId,
                    DoctorId = input.DoctorId,
                    PharmacyId = input.PharmacyId,
                    Note = input.Note,

                    Status = "Pending",
                    DateOfRequest = DateTime.Now,
                    DateOfResponse = null
                };

                db.PrescriptionRequests.Add(request);
                await db.SaveChangesAsync();

                return Results.Created($"/api/prescriptionrequests/{request.Id}", request);
            });

            // GET: aanvragen per user
            app.MapGet("/api/prescriptionrequests", async (ApiDbContext db, int userId) =>
            {
                var items = await db.PrescriptionRequests
                    .Where(r => r.UserId == userId)
                    .Include(r => r.ApprovedPrescription)
                    .OrderByDescending(r => r.DateOfRequest)
                    .ToListAsync();

                return Results.Ok(items);
            });

            app.MapPost("/api/prescriptionrequests/{id:int}/approve",
                async (ApiDbContext db, int id, ApprovePrescription input) =>
            {
                var request = await db.PrescriptionRequests
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (request == null)
                    return Results.NotFound("Aanvraag niet gevonden.");

                if (request.Status != "Pending")
                    return Results.BadRequest("Alleen Pending aanvragen kunnen worden goedgekeurd.");

                // Recept maken
                var prescription = new Prescription
                {
                    UserId = request.UserId,
                    MedicationId = request.MedicationId,
                    DoctorId = request.DoctorId,
                    PharmacyId = request.PharmacyId,

                    DateOfPrescription = DateTime.Now,

                    Amount = input.Amount,
                    Unit = input.Unit,
                    RefillsRemaining = input.RefillsRemaining,

                    TakeCount = input.TakeCount,
                    TimeBetweenMinutes = input.TimeBetweenMinutes,
                    Instruction = input.Instruction
                };

                db.Prescriptions.Add(prescription);
                await db.SaveChangesAsync(); // zodat prescription.Id bestaat

                // Request updaten
                request.Status = "Approved";
                request.DateOfResponse = DateTime.Now;
                request.ApprovedPrescriptionId = prescription.Id;

                await db.SaveChangesAsync();

                // Teruggeven incl recept
                var result = await db.PrescriptionRequests
                    .Include(r => r.ApprovedPrescription)
                    .FirstAsync(r => r.Id == id);

                return Results.Ok(result);
            });

            app.MapPost("/api/prescriptionrequests/{id:int}/deny", async (ApiDbContext db, int id) =>
            {
                var request = await db.PrescriptionRequests.FirstOrDefaultAsync(r => r.Id == id);
                if (request == null)
                    return Results.NotFound("Aanvraag niet gevonden.");

                if (request.Status != "Pending")
                    return Results.BadRequest("Alleen Pending aanvragen kunnen worden afgekeurd.");

                request.Status = "Denied";
                request.DateOfResponse = DateTime.Now;

                await db.SaveChangesAsync();
                
                var result = await db.PrescriptionRequests
                    .Include(r => r.ApprovedPrescription)
                    .FirstAsync(r => r.Id == id);

                return Results.Ok(result);

            });

            app.Run();
        }
    }
}
