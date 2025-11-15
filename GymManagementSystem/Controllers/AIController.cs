using GymManagementSystem.Data;
using GymManagementSystem.Models;
using GymManagementSystem.Services; // لاستخدام خدمة AI
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// تقييد الوصول للأعضاء المسجلين فقط
[Authorize(Roles = "Member, Admin")]
public class AIController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAIService _aiService;
    private readonly UserManager<IdentityUser> _userManager;

    public AIController(ApplicationDbContext context, IAIService aiService, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _aiService = aiService;
        _userManager = userManager;
    }

    // GET: /AI/Profile
    // لعرض واجهة إدخال بيانات المستخدم أو تعديلها
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // محاولة جلب ملف البيانات الحالي للمستخدم
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.MemberId == userId);

        if (profile == null)
        {
            // إذا لم يكن لديه ملف بيانات، نبدأ بملف جديد
            profile = new UserProfile { MemberId = userId };
        }

        return View(profile);
    }

    // POST: /AI/SaveProfile
    // لحفظ بيانات المستخدم واستدعاء خدمة AI
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProfile(UserProfile profile)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        profile.MemberId = userId;

        // إزالة التحقق من ModelState لكيانات التنقل التي لم نملأها
        ModelState.Remove("Member");

        if (ModelState.IsValid)
        {
            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.MemberId == userId);

            if (existingProfile == null)
            {
                // إذا كان ملف البيانات غير موجود، قم بإضافته
                _context.Add(profile);
            }
            else
            {
                // إذا كان موجودًا، قم بتحديثه
                existingProfile.HeightCm = profile.HeightCm;
                existingProfile.WeightKg = profile.WeightKg;
                existingProfile.Age = profile.Age;
                existingProfile.FitnessGoal = profile.FitnessGoal;
                _context.Update(existingProfile);
            }

            await _context.SaveChangesAsync();

            // التوجيه إلى صفحة عرض الخطة بعد الحفظ الناجح
            return RedirectToAction("GeneratePlan");
        }

        return View("Profile", profile);
    }

    // GET: /AI/GeneratePlan
    // لعرض الخطة التي تم توليدها بواسطة الذكاء الاصطناعي
    public async Task<IActionResult> GeneratePlan()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // يجب جلب أحدث بيانات للمستخدم
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.MemberId == userId);

        if (profile == null)
        {
            // إذا لم يكمل المستخدم بياناته، نطلب منه ذلك أولاً
            return RedirectToAction("Profile");
        }

        // استدعاء خدمة الذكاء الاصطناعي
        ViewData["WorkoutPlan"] = await _aiService.GenerateWorkoutPlanAsync(profile);

        return View(profile);
    }
}