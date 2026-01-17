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
            return await Database.Users.ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await Database.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            Database.Users.Add(user);
            await Database.SaveChangesAsync();
        }

        // Registratie - Email Check
        public async Task<bool> EmailBestaatAsync(string email)
        {
            return await Database.Users.AnyAsync(u => u.Email == email);
        }
    }
}
