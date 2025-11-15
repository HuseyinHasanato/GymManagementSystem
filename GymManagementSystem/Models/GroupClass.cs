using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GymManagementSystem.Models
{
    public class GroupClass
    {
        [Key]
        public int GroupClassId { get; set; }

        [Required(ErrorMessage = "Ders adı zorunludur.")]
        [Display(Name = "Ders Adı")]
        [StringLength(100)]
        public required string Name { get; set; }

        [Display(Name = "Açıklama")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Ders süresi zorunludur.")]
        [Range(15, 180, ErrorMessage = "Süre 15 ile 180 dakika arasında olmalıdır.")]
        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Maksimum kapasite zorunludur.")]
        [Range(1, 100, ErrorMessage = "Kapasite 1 ile 100 arasında olmalıdır.")]
        [Display(Name = "Maksimum Kapasite")]
        public int MaxCapacity { get; set; }

        // حساب وقت الانتهاء تلقائياً (خاصية للقراءة فقط)
        [Display(Name = "Bitiş Saati")]
        public DateTime EndTime => StartTime.AddMinutes(DurationMinutes);

        // --- علاقة المدرب ---
        [Required(ErrorMessage = "Lütfen bir eğitmen seçin.")]
        [Display(Name = "Eğitmen")]
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer? Trainer { get; set; }

        // --- العلاقات مع المشتركين والحجوزات ---
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
    }
}