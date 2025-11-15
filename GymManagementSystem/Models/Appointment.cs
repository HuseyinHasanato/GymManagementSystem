using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // مطلوب للربط بجدول AspNetUsers

namespace GymManagementSystem.Models
{
    public class Appointment
    {
        // المفتاح الأساسي للموعد
        public int AppointmentId { get; set; }

        // ******* الربط بالمستخدم (العضو الذي حجز الموعد) *******
        // المفتاح الأجنبي للمستخدم (العضو) - يجب أن يكون string لأنه يتطابق مع Id في IdentityUser
        public string MemberId { get; set; }

        // خاصية التنقل للمستخدم (IdentityUser)
        [ForeignKey("MemberId")]
        [Display(Name = "العضو الحاجز")]
        public virtual IdentityUser Member { get; set; }
        // *******************************************************

        // ******* الربط بالفئة/الخدمة (GroupClass) *******
        [Required]
        [Display(Name = "الفئة/الخدمة المحجوزة")]
        // هذا الربط يضمن أن الحجز متعلق بنوع خدمة معينة (Fitness, Yoga) 
        public int GroupClassId { get; set; }

        // خاصية التنقل للفئة
        [ForeignKey("GroupClassId")]
        public virtual GroupClass Class { get; set; }
        // *************************************************

        // تاريخ ووقت بدء الموعد - مطلوب لتطبيق منطق "التحقق من التوفر" 
        [Required(ErrorMessage = "تاريخ ووقت البدء مطلوب.")]
        [Display(Name = "تاريخ ووقت البدء")]
        public DateTime StartTime { get; set; }

        // تاريخ ووقت انتهاء الموعد (يتم حسابه بناءً على مدة الفئة/الخدمة)
        [Required(ErrorMessage = "وقت الانتهاء مطلوب.")]
        [Display(Name = "وقت الانتهاء")]
        public DateTime EndTime { get; set; }

        // حالة الموعد - مطلوب لتحقيق "آلية الموافقة على المواعيد" 
        [Display(Name = "تم التأكيد؟")]
        public bool IsConfirmed { get; set; } = false; // القيمة الافتراضية: بانتظار التأكيد

        [Display(Name = "ملاحظات العضو")]
        public string Notes { get; set; }
    }
}