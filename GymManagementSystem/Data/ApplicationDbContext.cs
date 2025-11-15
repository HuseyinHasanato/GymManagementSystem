using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // جداول البيانات الأساسية
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<GroupClass> GroupClasses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. إعداد علاقة UserProfile مع IdentityUser
            // يضمن أن ملف البيانات الشخصية مرتبط بمستخدم واحد فقط
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasOne(up => up.Member)
                      .WithMany()
                      .HasForeignKey(up => up.MemberId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); // إذا حُذف المستخدم، يُحذف ملفه الشخصي تلقائياً
            });

            // 2. إعداد مفتاح مركب لـ ClassEnrollment (سجل التسجيل في الحصص)
            // يمنع المستخدم من التسجيل في نفس الحصة مرتين
            builder.Entity<ClassEnrollment>()
                .HasKey(ce => new { ce.GroupClassId, ce.UserId });

            // 3. تنظيم علاقة الحصص الجماعية بالمدربين
            builder.Entity<GroupClass>()
                .HasOne(gc => gc.Trainer)
                .WithMany(t => t.GroupClasses)
                .HasForeignKey(gc => gc.TrainerId)
                .OnDelete(DeleteBehavior.Restrict); // يمنع حذف مدرب إذا كان لديه حصص مجدولة (للأمان)

            // 4. إعداد علاقات ClassEnrollment مع المستخدم والحصة
            builder.Entity<ClassEnrollment>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ce => ce.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}