using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen eğitmenin adını ve soyadını girin.")]
        [StringLength(100)]
        [Display(Name = "Adı Soyadı")]
        public required string FullName { get; set; }

        [StringLength(150)]
        [Display(Name = "Uzmanlık Alanı")]
        public string? Specialty { get; set; }

        [Display(Name = "Fotoğraf URL")]
        public string? ImageUrl { get; set; }

        public ICollection<GroupClass> GroupClasses { get; set; } = new List<GroupClass>();
    }
}