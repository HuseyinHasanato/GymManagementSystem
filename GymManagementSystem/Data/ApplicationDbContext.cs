using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;

namespace GymManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // الجداول السابقة
        public DbSet<GymService> GymServices { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        // --- الجدول الجديد للذكاء الاصطناعي ---
        public DbSet<AIGymProfile> AIGymProfiles { get; set; }
        // -------------------------------------
    }
}