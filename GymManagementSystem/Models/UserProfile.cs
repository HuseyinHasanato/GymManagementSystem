using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string MemberId { get; set; } = string.Empty;

        [ForeignKey("MemberId")]
        public virtual IdentityUser? Member { get; set; }

        [Required(ErrorMessage = "Boy alanı zorunludur.")]
        [Range(50, 250, ErrorMessage = "Boy 50 ile 250 cm arasında olmalıdır.")]
        [Display(Name = "Boy (cm)")]
        public int HeightCm { get; set; }

        [Required(ErrorMessage = "Kilo alanı zorunludur.")]
        [Range(10, 300, ErrorMessage = "Kilo 10 ile 300 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        [Column(TypeName = "decimal(18, 2)")] // دقة عالية للوزن في قاعدة البيانات
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "Yaş alanı zorunludur.")]
        [Range(10, 100, ErrorMessage = "Yaş 10 ile 100 arasında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Lütfen bir hedef seçin.")]
        [Display(Name = "Fitness Hedefi")]
        [StringLength(200)]
        public string FitnessGoal { get; set; } = "Kilo Vermek"; // قيمة افتراضية لتجنب الخطأ
    }
}