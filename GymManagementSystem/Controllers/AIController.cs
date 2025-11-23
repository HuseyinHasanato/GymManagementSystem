using GymManagementSystem.Data;
using GymManagementSystem.Models;
using GymManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymManagementSystem.Controllers
{
    [Authorize(Roles = "Member, Admin")]
    public class AIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIService _aiService;
        private readonly ILogger<AIController> _logger;

        public AIController(
            ApplicationDbContext context,
            IAIService aiService,
            ILogger<AIController> logger)
        {
            _context = context;
            _aiService = aiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.MemberId == userId);

            if (profile == null)
            {
                profile = new UserProfile { MemberId = userId };
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProfile(UserProfile profile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            profile.MemberId = userId;
            ModelState.Remove("Member");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.MemberId == userId);

                    if (existingProfile == null)
                    {
                        _context.Add(profile);
                    }
                    else
                    {
                        existingProfile.HeightCm = profile.HeightCm;
                        existingProfile.WeightKg = profile.WeightKg;
                        existingProfile.Age = profile.Age;
                        existingProfile.FitnessGoal = profile.FitnessGoal;
                        _context.Update(existingProfile);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(GeneratePlan));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Profil kaydetme hatası.");
                    ModelState.AddModelError("", "Veritabanı Hatası: " + ex.Message);
                }
            }
            return View("Profile", profile);
        }

        [HttpGet]
        public async Task<IActionResult> GeneratePlan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // تأكد من جلب البيانات الأساسية
            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.MemberId == userId);

            if (profile == null) return RedirectToAction(nameof(Profile));

            try
            {
                // استدعاء الخدمة مع مهلة زمنية بسيطة محاكة
                var workoutPlan = await _aiService.GenerateWorkoutPlanAsync(profile);

                if (string.IsNullOrWhiteSpace(workoutPlan))
                {
                    ViewBag.WorkoutPlan = "⚠️ Plan oluşturulamadı. Lütfen bilgilerinizi kontrol edip tekrar deneyin.";
                }
                else
                {
                    // نستخدم هذا المتغير لعرض النص في الـ View
                    ViewBag.WorkoutPlan = workoutPlan;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Servis Hatası");
                ViewBag.WorkoutPlan = $"❌ Bağlantı Hatası: API şu anda yanıt vermiyor. (Detay: {ex.Message})";
            }

            return View(profile); // نرسل البروفايل لعرض البيانات الشخصية بجانب الخطة
        }
    }
}