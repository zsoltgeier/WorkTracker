using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Models;

namespace WorkTracker.Data
{
    public class WorkTrackerDbContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbFilePath = Path.Combine(baseDirectory, "WorkTrackerDb.db");
            string connectionString = $"Data Source={dbFilePath}";
            // Configure SQLite as the database provider
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}
