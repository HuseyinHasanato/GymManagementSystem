using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // AspNetUsers tablosuyla bağlantı için gerekli

namespace GymManagementSystem.Models
{
    public class Appointment
    {
        // Randevunun birincil anahtarı
        public int AppointmentId { get; set; }

        // ******* Kullanıcıya Bağlantı (Randevuyu Alan Üye) *******
        // Kullanıcının (Üyenin) yabancı anahtarı - IdentityUser'daki Id ile eşleştiği için string olmalıdır
        public string MemberId { get; set; }

        // Kullanıcının (IdentityUser) navigasyon özelliği
        [ForeignKey("MemberId")]
        [Display(Name = "Randevu Alan Üye")]
        public virtual IdentityUser Member { get; set; }
        // *******************************************************

        // ******* Sınıf/Hizmet Bağlantısı (GroupClass) *******
        [Required]
        [Display(Name = "Rezerve Edilen Sınıf/Hizmet")]
        // Bu bağlantı, rezervasyonun belirli bir hizmet türüyle (Fitness, Yoga) ilgili olmasını sağlar
        public int GroupClassId { get; set; }

        // Sınıfın navigasyon özelliği
        [ForeignKey("GroupClassId")]
        public virtual GroupClass Class { get; set; }
        // *************************************************

        // Randevunun başlangıç tarihi ve saati - "Müsaitlik Kontrolü" mantığı için zorunlu
        [Required(ErrorMessage = "Başlangıç tarihi ve saati zorunludur.")]
        [Display(Name = "Başlangıç Tarihi ve Saati")]
        public DateTime StartTime { get; set; }

        // Randevunun bitiş tarihi ve saati (Sınıfın/Hizmetin süresine göre hesaplanır)
        [Required(ErrorMessage = "Bitiş zamanı zorunludur.")]
        [Display(Name = "Bitiş Zamanı")]
        public DateTime EndTime { get; set; }

        // Randevu durumu - "Randevu Onay Mekanizması" için zorunlu
        [Display(Name = "Onaylandı Mı?")]
        public bool IsConfirmed { get; set; } = false; // Varsayılan değer: Onay bekliyor

        [Display(Name = "Üye Notları")]
        public string Notes { get; set; }
    }
}