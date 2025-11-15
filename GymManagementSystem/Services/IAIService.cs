using GymManagementSystem.Models;

namespace GymManagementSystem.Services
{
    public interface IAIService
    {
        /// <summary>
        /// توليد خطة تدريب مخصصة بناءً على البيانات البدنية للمشترك.
        /// </summary>
        Task<string> GenerateWorkoutPlanAsync(UserProfile profile);
    }
}