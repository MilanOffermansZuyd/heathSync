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

        public async Task<List<User>> GetUsersAsync()
        {
            return await Database.Gebruikers.ToListAsync();
        }
    }
}
