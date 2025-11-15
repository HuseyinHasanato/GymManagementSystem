using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GymManagementSystem.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        // إضافة ? تجعل الحقل يقبل القيمة الفارغة أثناء التحقق الأولي
        // إضافة [ValidateNever] تمنع النظام من المطالبة به في الـ ModelState
        [ValidateNever]
        public string? MemberId { get; set; }

        [ForeignKey("MemberId")]
        [ValidateNever]
        public virtual IdentityUser? Member { get; set; }

        [Required(ErrorMessage = "Lütfen bir ders seçiniz.")]
        public int GroupClassId { get; set; }

        [ForeignKey("GroupClassId")]
        [ValidateNever]
        public virtual GroupClass? Class { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        public DateTime StartTime { get; set; }

        // جعلنا هذا الحقل لا يتطلب إدخال مباشر من الواجهة
        [ValidateNever]
        public DateTime EndTime { get; set; }

        public bool IsConfirmed { get; set; } = false;

        public string? Notes { get; set; }
    }
}