using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models
{
    // Eğitmen veritabanı modelini temsil eder
    public class Trainer
    {
        // Birincil Anahtar
        [Key]
        public int Id { get; set; }

        // Eğitmenin Tam Adı (Zorunlu Alan)
        [Required(ErrorMessage = "Lütfen eğitmenin adını ve soyadını girin.")]
        [StringLength(100, ErrorMessage = "Adı Soyadı alanı maksimum 100 karakter olmalıdır.")]
        [Display(Name = "Adı Soyadı")]
        public required string FullName { get; set; }

        // Eğitmenin Uzmanlık Alanı (Opsiyonel)
        [StringLength(150, ErrorMessage = "Uzmanlık Alanı maksimum 150 karakter olmalıdır.")]
        [Display(Name = "Uzmanlık Alanı")]
        public string? Specialty { get; set; }

        // Eğitmen Fotoğrafının URL'si (Opsiyonel)
        [Display(Name = "Fotoğraf URL")]
        public string? ImageUrl { get; set; }

        // Navigasyon Özelliği: Bu eğitmenin verdiği grup derslerini tutar
        // Bir Eğitmen birden çok ders verebilir (Çoğul İlişki)
        public ICollection<GroupClass> GroupClasses { get; set; } = new List<GroupClass>();
    }
}