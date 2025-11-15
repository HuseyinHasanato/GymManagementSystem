using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models
{
    public class ClassEnrollment
    {
        // تم حذف [Key] public int Id { get; set; }  

        [Display(Name = "Kayıt Tarihi")]
        public DateTime EnrollmentDate { get; set; }

        // المفتاح الخارجي للحصة
        [Required] // يجب أن يكون مطلوبًا لأنه جزء من المفتاح الأساسي المركب
        public int GroupClassId { get; set; }
        [ForeignKey("GroupClassId")]
        public GroupClass? GroupClass { get; set; }

        // المفتاح الخارجي للمستخدم (النوع الصحيح هو string)
        // 🚨 التصحيح: إضافة Required و MaxLength(448) لتصحيح حجم المفتاح المركب (الذي تجاوز 900 بايت)
        [Required]
        [MaxLength(448)]
        public string? UserId { get; set; }

        // (يجب أن تكون خاصية التنقل للمستخدم محذوفة كما هي)
        // public IdentityUser? User { get; set; } 
    }
}