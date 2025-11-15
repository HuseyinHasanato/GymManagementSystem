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

        // تعريف جداول الـ DbSet
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<GroupClass> GroupClasses { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // هذا السطر إلزامي لتشغيل جداول Identity (المستخدمين والأدوار)
            base.OnModelCreating(builder);

            // 1. تعريف المفتاح المركب (Composite Key) لجدول ClassEnrollment
            // هذا يحل محل خاصية [Key] public int Id المحذوفة من الموديل
            builder.Entity<ClassEnrollment>()
                .HasKey(ce => new { ce.GroupClassId, ce.UserId });

            // 2. تعريف العلاقة مع GroupClass (حصة المجموعة)
            builder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.GroupClass)
                .WithMany(gc => gc.ClassEnrollments)
                .HasForeignKey(ce => ce.GroupClassId)
                // يمنع حذف الحصة إذا كان هناك حجوزات (للحفاظ على تكامل البيانات)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. تعريف العلاقة مع IdentityUser (المستخدم)
            // هذا هو التصحيح الذي يمنع ظهور خطأ UserId1
            builder.Entity<ClassEnrollment>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ce => ce.UserId)
                // نستخدم IsRequired(false) ليتوافق مع string? UserId
                .IsRequired(false)
                // يمنع حذف المستخدم إذا كان لديه حجوزات
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}