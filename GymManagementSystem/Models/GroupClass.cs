using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // ICollection için gerekli

namespace GymManagementSystem.Models
{
    // Spor Salonunda sunulan grup derslerini veya hizmetleri temsil eder
    public class GroupClass
    {
        [Key]
        public int GroupClassId { get; set; }

        // Ders Adı (Zorunlu)
        [Required(ErrorMessage = "Ders adı zorunludur.")]
        [Display(Name = "Ders Adı")]
        public required string Name { get; set; } // 'required' anahtar kelimesi eklendi

        // Dersin Detaylı Açıklaması
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // Dersin Başlangıç Saati (Hangi saatte başladığı)
        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        // Dikkat: Bu alan, tekrar eden dersler için sadece zamanı tutabilir veya belirli bir günü.
        public DateTime StartTime { get; set; }

        // Dersin alabileceği maksimum kişi sayısı
        [Required(ErrorMessage = "Maksimum kapasite zorunludur.")]
        [Range(1, 100, ErrorMessage = "Kapasite 1 ile 100 arasında olmalıdır.")]
        [Display(Name = "Maksimum Kapasite")]
        public int MaxCapacity { get; set; }

        // --- Eğitmen Bağlantısı (Foreign Key) ---
        [Display(Name = "Eğitmen")]
        // Hata önlemek için 'Trainer' yerine 'TrainerId' kullanmak daha güvenli:
        [ForeignKey("TrainerId")]
        public int TrainerId { get; set; }

        // Navigasyon Özelliği: Eğitmen (Trainer) nesnesine erişim sağlar
        public Trainer? Trainer { get; set; }

        // Ders Süresi (AppointmentsController'da kullanıldı)
        [Required(ErrorMessage = "Ders süresi zorunludur.")]
        [Range(15, 180, ErrorMessage = "Süre 15 ile 180 dakika arasında olmalıdır.")]
        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; }

        // --- Randevulara Navigasyon Özelliği ---
        // Randevular (Hangi üyelerin bu derse kayıt yaptığını gösterir)
        // Eğer 'Appointment' modelini kullandıysanız, bu kısmı düzeltmelisiniz.
        public ICollection<Appointment>? Appointments { get; set; } // Appointment kullanıldığını varsayıyoruz.

        // NOT: Eğer GroupClass modeli, üyeler arasındaki randevular yerine 
        // sürekli dersleri temsil ediyorsa, bu modelin yapısı uygundur.
    }
}