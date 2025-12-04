using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Display(Name = "تاريخ الحجز")]
        public DateTime AppointmentDate { get; set; }

        public bool IsConfirmed { get; set; } = false; // حالة الموافقة

        // ربط الحجز بالعضو (المستخدم)
        public string MemberId { get; set; }
        public IdentityUser? Member { get; set; }

        // ربط الحجز بالخدمة
        public int GymServiceId { get; set; }
        public GymService? GymService { get; set; }

        // ربط الحجز بالمدرب
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }
    }
}