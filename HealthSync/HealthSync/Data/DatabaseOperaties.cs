using HealthSync.Models;
using HealthSync.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSync.Data
{
    public class DatabaseOperaties
    {
        private readonly HealthSyncContext Database;

        public DatabaseOperaties(HealthSyncContext database)
        {
            Database = database;
        }

        // User
        public async Task<List<User>> GetUsersAsync()
        {
            return await Database.Users
                .Include(u => u.HealthData)
                .Include(u => u.Prescriptions)
                .Include(u => u.PrescriptionRequests)
                .Include(u => u.EmergencyContacts)
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await Database.Users
                .Include(u => u.HealthData)
                .Include(u => u.Prescriptions)
                .Include(u => u.PrescriptionRequests)
                .Include(u => u.EmergencyContacts)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            Database.Users.Add(user);
            await Database.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            Database.Users.Update(user);
            await Database.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            Database.Users.Remove(user);
            await Database.SaveChangesAsync();
        }

        // EmergencyContact
        public async Task<List<EmergencyContact>> GetEmergencyContactAsync()
        {
            return await Database.EmergencyContacts.ToListAsync();
        }

        public async Task<List<EmergencyContact>> GetEmergencyContactsByUserIdAsync(int id)
        {
            return await Database.EmergencyContacts
                .Where(u => u.UserId == id)
                .Select(x => x)
                .ToListAsync();
        }

        public async Task AddEmergencyContactAsync(EmergencyContact emergencyContact)
        {
            Database.EmergencyContacts.Add(emergencyContact);
            await Database.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(EmergencyContact emergencyContact)
        {
            Database.EmergencyContacts.Update(emergencyContact);
            await Database.SaveChangesAsync();
        }

        public async Task DeleteEmergencyContactAsync(EmergencyContact emergencyContact)
        {
            Database.EmergencyContacts.Remove(emergencyContact);
            await Database.SaveChangesAsync();
        }

        // Medication
        public async Task<List<Medication>> GetMedicationsAsync()
        {
            return await Database.Medications.OrderBy(m => m.Name).ThenBy(m => m.Strength).ToListAsync();
        }

        public async Task<Medication?> GetMedicationByIdAsync(int id)
        {
            return await Database.Medications.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddMedicationAsync(Medication medication)
        {
            Database.Medications.Add(medication);
            await Database.SaveChangesAsync();
        }

        public async Task UpdateMedicationAsync(Medication medication)
        {
            Database.Medications.Update(medication);
            await Database.SaveChangesAsync();
        }

        public async Task DeleteMedicationAsync(Medication medication)
        {
            Database.Medications.Remove(medication);
            await Database.SaveChangesAsync();
        }

        // Prescription
        public async Task<List<Prescription>> GetPrescriptionsAsync()
        {
            return await Database.Prescriptions
                .Include(p => p.Medication)
                .Include(p => p.Doctor)
                .Include(p => p.Pharmacy)
                .ToListAsync();
        }

        public async Task<List<Prescription>> GetPrescriptionsByUserIdAsync(int userId)
        {
            return await Database.Prescriptions
                .Where(p => p.UserId == userId)
                .Include(p => p.Medication)
                .Include(p => p.Doctor)
                .Include(p => p.Pharmacy)
                .ToListAsync();
        }

        public async Task AddPrescriptionAsync(Prescription prescription)
        {
            Database.Prescriptions.Add(prescription);
            await Database.SaveChangesAsync();
        }

        public async Task UpdatePrescriptionAsync(Prescription prescription)
        {
            Database.Prescriptions.Update(prescription);
            await Database.SaveChangesAsync();
        }

        public async Task DeletePrescriptionAsync(Prescription prescription)
        {
            Database.Prescriptions.Remove(prescription);
            await Database.SaveChangesAsync();
        }

        // PrescriptionRequest
        public async Task<List<PrescriptionRequest>> GetPrescriptionRequestsAsync()
        {
            return await Database.PrescriptionRequests
                .Include(r => r.Medication)
                .Include(r => r.Doctor)
                .Include(r => r.Pharmacy)
                .Include(r => r.ApprovedPrescription)
                    .ThenInclude(p => p.Medication)
                .ToListAsync();
        }

        public async Task<List<PrescriptionRequest>> GetPrescriptionRequestsByUserIdAsync(int userId)
        {
            return await Database.PrescriptionRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Medication)
                .Include(r => r.Doctor)
                .Include(r => r.Pharmacy)
                .Include(r => r.ApprovedPrescription)
                    .ThenInclude(p => p.Medication)
                .OrderByDescending(r => r.DateOfRequest)
                .ToListAsync();
        }

        public async Task AddPrescriptionRequestAsync(PrescriptionRequest request)
        {
            Database.PrescriptionRequests.Add(request);
            await Database.SaveChangesAsync();
        }

        public async Task UpdatePrescriptionRequestAsync(PrescriptionRequest request)
        {
            Database.PrescriptionRequests.Update(request);
            await Database.SaveChangesAsync();
        }
        public async Task DeletePrescriptionRequestAsync(PrescriptionRequest request)
        {
            Database.PrescriptionRequests.Remove(request);
            await Database.SaveChangesAsync();
        }

        // HealthData
        public async Task<List<HealthData>> GetHealthDataAsync()
        {
            return await Database.HealthDatas
                .Include(h => h.User)
                .ToListAsync();
        }

        public async Task<HealthData?> GetHealthDataByUserIdAsync(int userId)
        {
            return await Database.HealthDatas
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.UserId == userId);
        }

        public async Task AddHealthDataAsync(HealthData healthData)
        {
            Database.HealthDatas.Add(healthData);
            await Database.SaveChangesAsync();
        }

        public async Task UpdateHealthDataAsync(HealthData healthData)
        {
            Database.HealthDatas.Update(healthData);
            await Database.SaveChangesAsync();
        }

        public async Task DeleteHealthDataAsync(HealthData healthData)
        {
            Database.HealthDatas.Remove(healthData);
            await Database.SaveChangesAsync();
        }

        // Notification
        public async Task<List<Notification>> GetNotificationsAsync()
        {
            return await Database.Notifications
                .Include(n => n.Prescription)
                .ThenInclude(p => p.Medication)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetNotificationsByPrescriptionIdAsync(int prescriptionId)
        {
            return await Database.Notifications
                .Where(n => n.PrescriptionId == prescriptionId)
                .Include(n => n.Prescription)
                .ThenInclude(p => p.Medication)
                .ToListAsync();
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            Database.Notifications.Add(notification);
            await Database.SaveChangesAsync();
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            Database.Notifications.Update(notification);
            await Database.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(Notification notification)
        {
            Database.Notifications.Remove(notification);
            await Database.SaveChangesAsync();
        }

        // Doctor
        public async Task<List<Doctor>> GetDoctorsAsync()
        {
            return await Database.Doctors
                .OrderBy(d => d.LastName).ThenBy(d => d.FirstName)
                .ToListAsync();
        }

        // Pharmacy
        public async Task<List<Pharmacy>> GetPharmaciesAsync()
        {
            return await Database.Pharmacies
                .OrderBy(p => p.PharmacyName)
                .ToListAsync();
        }


        // Registratie - Email Check
        public async Task<bool> EmailBestaatAsync(string email)
        {
            return await Database.Users.AnyAsync(u => u.Email == email);
        }

        // API Sync
        public async Task SyncPrescriptionDataFromApiAsync(ApiService api, int userId)
        {
            var remoteRequests = await api.GetPrescriptionRequestsAsync(userId);

            var localRequests = await GetPrescriptionRequestsByUserIdAsync(userId);
            var localPrescriptions = await GetPrescriptionsByUserIdAsync(userId);

            var localByRemoteId = localRequests
                .Where(r => r.RemoteId.HasValue)
                .GroupBy(r => r.RemoteId!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var localByClientId = localRequests
                .GroupBy(r => r.ClientRequestId)
                .ToDictionary(g => g.Key, g => g.First());

            var localPrescriptionByRemoteId = localPrescriptions
                .Where(p => p.RemoteId.HasValue)
                .GroupBy(p => p.RemoteId!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var remote in remoteRequests)
            {
                PrescriptionRequest? local = null;

                if (remote.RemoteId.HasValue && localByRemoteId.TryGetValue(remote.RemoteId.Value, out var hitRemote))
                    local = hitRemote;
                else if (localByClientId.TryGetValue(remote.ClientRequestId, out var hitClient))
                    local = hitClient;

                if (local == null)
                {
                    local = new PrescriptionRequest
                    {
                        ClientRequestId = remote.ClientRequestId,
                        RemoteId = remote.RemoteId,

                        UserId = remote.UserId,
                        MedicationId = remote.MedicationId,
                        DoctorId = remote.DoctorId,
                        PharmacyId = remote.PharmacyId,

                        Note = remote.Note,
                        Status = remote.Status,
                        DateOfRequest = remote.DateOfRequest,
                        DateOfResponse = remote.DateOfResponse,

                        ApprovedPrescriptionRemoteId = remote.ApprovedPrescriptionRemoteId
                    };

                    await AddPrescriptionRequestAsync(local);

                    if (local.RemoteId.HasValue)
                        localByRemoteId[local.RemoteId.Value] = local;
                    localByClientId[local.ClientRequestId] = local;
                }
                else
                {
                    local.RemoteId = remote.RemoteId;
                    local.Status = remote.Status;
                    local.DateOfRequest = remote.DateOfRequest;
                    local.DateOfResponse = remote.DateOfResponse;
                    local.Note = remote.Note;

                    local.ApprovedPrescriptionRemoteId = remote.ApprovedPrescriptionRemoteId;

                    if (remote.Status != Models.Enums.RequestStatus.Approved)
                        local.ApprovedPrescriptionId = null;

                    await UpdatePrescriptionRequestAsync(local);
                }

                if (remote.Status == Models.Enums.RequestStatus.Approved && remote.ApprovedPrescription != null)
                {
                    var remotePrescriptionId =
                        remote.ApprovedPrescriptionRemoteId
                        ?? remote.ApprovedPrescription.RemoteId;

                    if (remotePrescriptionId.HasValue)
                    {
                        if (!localPrescriptionByRemoteId.TryGetValue(remotePrescriptionId.Value, out var localPrescription))
                        {
                            localPrescription = new Prescription
                            {
                                RemoteId = remotePrescriptionId.Value,

                                UserId = remote.ApprovedPrescription.UserId,
                                MedicationId = remote.ApprovedPrescription.MedicationId,
                                DoctorId = remote.ApprovedPrescription.DoctorId,
                                PharmacyId = remote.ApprovedPrescription.PharmacyId,

                                DateOfPrescription = remote.ApprovedPrescription.DateOfPrescription,
                                Amount = remote.ApprovedPrescription.Amount,
                                Unit = remote.ApprovedPrescription.Unit,
                                RefillsRemaining = remote.ApprovedPrescription.RefillsRemaining,
                                TakeCount = remote.ApprovedPrescription.TakeCount,
                                TimeBetweenMinutes = remote.ApprovedPrescription.TimeBetweenMinutes,
                                Instruction = remote.ApprovedPrescription.Instruction
                            };

                            await AddPrescriptionAsync(localPrescription);
                            localPrescriptionByRemoteId[remotePrescriptionId.Value] = localPrescription;
                        }
                        else
                        {
                            localPrescription.DateOfPrescription = remote.ApprovedPrescription.DateOfPrescription;
                            localPrescription.Amount = remote.ApprovedPrescription.Amount;
                            localPrescription.Unit = remote.ApprovedPrescription.Unit;
                            localPrescription.RefillsRemaining = remote.ApprovedPrescription.RefillsRemaining;
                            localPrescription.TakeCount = remote.ApprovedPrescription.TakeCount;
                            localPrescription.TimeBetweenMinutes = remote.ApprovedPrescription.TimeBetweenMinutes;
                            localPrescription.Instruction = remote.ApprovedPrescription.Instruction;

                            await UpdatePrescriptionAsync(localPrescription);
                        }

                        local.ApprovedPrescriptionId = localPrescription.Id;
                        local.ApprovedPrescriptionRemoteId = remotePrescriptionId.Value;
                        await UpdatePrescriptionRequestAsync(local);
                    }
                }
            }
        }

    }
}
