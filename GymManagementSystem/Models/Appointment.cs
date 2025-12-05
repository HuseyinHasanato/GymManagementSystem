using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Randevu Tarihi")] // كانت: تاريخ الحجز
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Onay Durumu")] // كانت: IsConfirmed
        public bool IsConfirmed { get; set; } = false;

        // --- العلاقات ---

        [Display(Name = "Üye")] // كانت: Member
        public string? MemberId { get; set; }

        [ForeignKey("MemberId")]
        [Display(Name = "Üye")]
        public IdentityUser? Member { get; set; }

        [Display(Name = "Hizmet")] // كانت: GymService
        public int GymServiceId { get; set; }

        [ForeignKey("GymServiceId")]
        [Display(Name = "Hizmet")]
        public GymService? GymService { get; set; }

        [Display(Name = "Eğitmen")] // كانت: Trainer
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        [Display(Name = "Eğitmen")]
        public Trainer? Trainer { get; set; }
    }
}