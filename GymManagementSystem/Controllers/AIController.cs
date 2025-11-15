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

        // 1. عرض صفحة إدخال البيانات (الطول، الوزن، الهدف)
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

        // 2. حفظ البيانات والانتقال لتوليد الخطة
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
                    TempData["SuccessMessage"] = "Profiliniz güncellendi. Planınız hazırlanıyor...";

                    return RedirectToAction(nameof(GeneratePlan));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Profil kaydetme hatası.");
                    ModelState.AddModelError("", "Veritabanına kaydedilirken bir hata oluştu.");
                }
            }
            return View("Profile", profile);
        }

        // 3. الأكشن المسؤول عن استدعاء Gemini وعرض النتيجة
        [HttpGet]
        public async Task<IActionResult> GeneratePlan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // جلب البيانات بدون تتبع لتسريع الأداء
            var profile = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.MemberId == userId);

            if (profile == null)
            {
                return RedirectToAction(nameof(Profile));
            }

            try
            {
                _logger.LogInformation("Gemini AI Planı oluşturuluyor: {UserId}", userId);

                // استدعاء خدمة Gemini التي قمنا بإعدادها بمفتاح AIza...
                var workoutPlan = await _aiService.GenerateWorkoutPlanAsync(profile);

                if (string.IsNullOrEmpty(workoutPlan))
                {
                    ViewBag.WorkoutPlan = "⚠️ AI yanıt üretemedi. Lütfen API anahtarını kontrol edin.";
                }
                else
                {
                    // تمرير النص (Markdown) إلى الصفحة
                    ViewBag.WorkoutPlan = workoutPlan;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Plan hatası.");
                ViewBag.WorkoutPlan = $"❌ Hata: {ex.Message}";
            }

            return View(profile);
        }
    }
}