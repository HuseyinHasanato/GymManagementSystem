using GymManagementSystem.Models;

namespace GymManagementSystem.Services
{
    /// <summary>
    /// Yapay Zeka (AI) servisleri için arayüz.
    /// </summary>
    public interface IAIService
    {
        /// <summary>
        /// Kullanıcının fiziksel bilgilerine göre kişiselleştirilmiş bir antrenman ve beslenme planı oluşturur.
        /// </summary>
        /// <param name="profile">Kullanıcının boy, kilo, yaş ve hedef bilgilerini içeren profil verisi.</param>
        /// <returns>Yapay zeka tarafından oluşturulan Markdown formatında metin.</returns>
        Task<string> GenerateWorkoutPlanAsync(UserProfile profile);
    }
}