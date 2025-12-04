using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- هذا السطر مطلوب

namespace GymManagementSystem.Models
{
    public class GymService
    {
        public int Id { get; set; }

        [Required, Display(Name = "اسم الخدمة")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Display(Name = "المدة (دقيقة)")]
        public int Duration { get; set; }

        [Display(Name = "السعر")]
        // <--- هذا السطر الجديد يحل مشكلة التحذير
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}