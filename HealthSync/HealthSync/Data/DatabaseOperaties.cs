using HealthSync.Models;
using Microsoft.EntityFrameworkCore;
using System;
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
                .Include(u => u.EmergencyContacts)
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await Database.Users
                .Include(u => u.HealthData)
                .Include(u => u.Prescriptions)
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

        // Medication
        public async Task<List<Medication>> GetMedicationsAsync()
        {
            return await Database.Medications.ToListAsync();
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

        // Registratie - Email Check
        public async Task<bool> EmailBestaatAsync(string email)
        {
            return await Database.Users.AnyAsync(u => u.Email == email);
        }
    }
}
