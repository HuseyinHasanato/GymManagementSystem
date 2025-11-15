using GymManagementSystem.Models;

namespace GymManagementSystem.Services
{
    public interface IAIService
    {
        // دالة ترجع خطة التمارين كنص بناءً على بيانات المستخدم
        Task<string> GenerateWorkoutPlanAsync(UserProfile profile);

        // يمكن إضافة دالة لـ 'توقع شكل الجسم' إذا كنت تستخدم DALL-E أو نموذج صور (مطلوب في الوصف)
        // Task<string> GenerateBodyImageAsync(UserProfile profile, string prompt);
    }
}