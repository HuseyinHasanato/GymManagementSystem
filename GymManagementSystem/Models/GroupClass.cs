using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Models
{
    public class GroupClass
    {
        [Key]
        public int GroupClassId { get; set; }

        [Required(ErrorMessage = "Ders adı zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Maksimum kapasite zorunludur.")]
        [Display(Name = "Maksimum Kapasite")]
        public int MaxCapacity { get; set; }

        // --- المفتاح الأجنبي للمدرب (Trainer) ---
        [Display(Name = "Eğitmen")]
        [ForeignKey("Trainer")]
        public int TrainerId { get; set; }

        public Trainer? Trainer { get; set; }

        // --- ✅ الإضافة الجديدة: خاصية التنقل للحجوزات ---
        public ICollection<ClassEnrollment>? ClassEnrollments { get; set; }
    }
}