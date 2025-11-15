using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string MemberId { get; set; }

        [ForeignKey("MemberId")]
        public virtual IdentityUser Member { get; set; }

        [Required(ErrorMessage = "Boy zorunludur.")]
        [Range(50, 250, ErrorMessage = "Boy 50 ile 250 cm arasında olmalıdır.")]
        [Display(Name = "Boy (cm)")]
        public int HeightCm { get; set; }

        [Required(ErrorMessage = "Kilo zorunludur.")]
        [Range(10, 300, ErrorMessage = "Kilo 10 ile 300 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "Yaş zorunludur.")]
        [Range(10, 100, ErrorMessage = "Yaş 10 ile 100 yıl arasında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Display(Name = "Fitness Hedefi")]
        public string FitnessGoal { get; set; }
    }
}