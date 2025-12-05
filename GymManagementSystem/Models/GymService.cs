using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Models
{
    public class GymService
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Süre (Dakika)")]
        public int Duration { get; set; }

        [Display(Name = "Ücret (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}