using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Eğitmen adı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string Expertise { get; set; }

        [Display(Name = "Fotoğraf URL")]
        public string ImageUrl { get; set; }
    }
}