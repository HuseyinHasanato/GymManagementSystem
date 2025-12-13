using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Data
{
    // يجب أن يرث من IdentityDbContext<IdentityUser> لدعم نظام الأمان
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // جداول المشروع الأساسية
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<GroupClass> GroupClasses { get; set; }

        // جدول الحجوزات (الربط بين العضو والحصة)
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }

        // إعداد العلاقات (اختياري لكن مفضل لضمان عمل الـ Foreign Keys)
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // تعريف مفتاح مركب لمنع تسجيل العضو مرتين في نفس الحصة
            builder.Entity<ClassEnrollment>()
                .HasKey(ce => new { ce.GroupClassId, ce.UserId });

            // علاقة GroupClass -> ClassEnrollment
            builder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.GroupClass)
                .WithMany(gc => gc.ClassEnrollments)
                .HasForeignKey(ce => ce.GroupClassId);

            // علاقة IdentityUser -> ClassEnrollment
            // علاقة IdentityUser -> ClassEnrollment
            builder.Entity<ClassEnrollment>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ce => ce.UserId) // هنا نحدد المفتاح الأجنبي يدوياً
                .IsRequired(); // التأكيد على أنه مطلوب
        }
    }
}