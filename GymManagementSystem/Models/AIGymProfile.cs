using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models
{
    public class AIGymProfile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kilo bilgisi zorunludur.")]
        [Display(Name = "Kilo (kg)")]
        [Range(30, 300, ErrorMessage = "Kilo 30 ile 300 arasında olmalıdır.")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Boy bilgisi zorunludur.")]
        [Display(Name = "Boy (cm)")]
        [Range(100, 250, ErrorMessage = "Boy 100 ile 250 cm arasında olmalıdır.")]
        public double Height { get; set; }

        [Required(ErrorMessage = "Hedef seçimi zorunludur.")]
        [Display(Name = "Hedefiniz")]
        public string Goal { get; set; }

        [Display(Name = "AI İstemi")]
        public string? AIPrompt { get; set; }

        [Display(Name = "AI Tavsiyesi")]
        public string? AIResult { get; set; }

        public string MemberId { get; set; }
        [ForeignKey("MemberId")]
        public IdentityUser? Member { get; set; }
    }
}