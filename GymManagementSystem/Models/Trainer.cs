using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required, Display(Name = "اسم المدرب")]
        public string FullName { get; set; }

        [Display(Name = "التخصص")]
        public string Expertise { get; set; }

        public string ImageUrl { get; set; } // صورة المدرب
    }
}