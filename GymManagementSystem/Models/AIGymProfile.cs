using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // ضروري للتعرف على IdentityUser

namespace GymManagementSystem.Models
{
    public class AIGymProfile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الوزن مطلوب.")]
        [Display(Name = "الوزن (كجم)")]
        [Range(30, 300, ErrorMessage = "يجب أن يكون الوزن بين 30 و 300 كجم.")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "الطول مطلوب.")]
        [Display(Name = "الطول (سم)")]
        [Range(100, 250, ErrorMessage = "يجب أن يكون الطول بين 100 و 250 سم.")]
        public double Height { get; set; }

        [Required(ErrorMessage = "الهدف مطلوب.")]
        [Display(Name = "هدفك الرياضي")]
        public string Goal { get; set; }

        [Display(Name = "خطة الذكاء الاصطناعي المقترحة")]
        public string? AIPrompt { get; set; }

        [Display(Name = "النتيجة المقترحة")]
        public string? AIResult { get; set; }

        // الربط بالمستخدم
        public string MemberId { get; set; }

        [ForeignKey("MemberId")]
        public IdentityUser? Member { get; set; } // تم التصحيح هنا: IdentityUser بدلاً من ApplicationUser
    }
}