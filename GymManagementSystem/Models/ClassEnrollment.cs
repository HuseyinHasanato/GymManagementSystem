using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models
{
    public class ClassEnrollment
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime EnrollmentDate { get; set; }

      
        public int GroupClassId { get; set; }
        [ForeignKey("GroupClassId")]
        public GroupClass? GroupClass { get; set; }

       
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }
    }
}