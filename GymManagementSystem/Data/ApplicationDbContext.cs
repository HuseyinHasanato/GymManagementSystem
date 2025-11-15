using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Data
{
    // نستخدم IdentityUser كنوع عام (Generic Type)
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ***** جداول بيانات المشروع الأساسية *****
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<GroupClass> GroupClasses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        // ***** جدول مطلوب لتكامل الذكاء الاصطناعي (AI Integration) *****
        // [cite: 31]
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        // public DbSet<ClassEnrollment> ClassEnrollments { get; set; } // تم التعليق عليه مؤقتاً لعدم وضوح دوره

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // هذا السطر إلزامي لتشغيل جداول Identity (المستخدمين والأدوار)
            base.OnModelCreating(builder);

            // *****************************************************************
            // توضيح علاقة المفتاح الأجنبي لـ UserProfile
            // يضمن أن كل UserProfile مرتبط بـ IdentityUser واحد
            builder.Entity<UserProfile>()
                .HasOne(up => up.Member) // يربط بـ خاصية Member في UserProfile
                .WithMany()
                .HasForeignKey(up => up.MemberId)
                .IsRequired();
            // *****************************************************************


            // ملاحظة: إذا كنت تنوي استخدام ClassEnrollment، فيجب أن يكون لديه نموذج (Model) خاص به.
             // إذا كنت تستخدم ClassEnrollment، يجب أن يكون الكود كما يلي:
            builder.Entity<ClassEnrollment>()
                .HasKey(ce => new { ce.GroupClassId, ce.UserId });

            builder.Entity<ClassEnrollment>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ce => ce.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
            
        }
    }
}