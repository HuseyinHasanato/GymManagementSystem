using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GymManagementSystem.Data;
using GymManagementSystem.Models;

namespace GymManagementSystem.Controllers
{
    [Authorize] // يجب أن يكون عضواً مسجلاً
    public class AIGymController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AIGymController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. عرض الصفحة الرئيسية (إدخال البيانات أو عرض النتيجة السابقة)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // البحث عما إذا كان للمستخدم ملف سابق
            var profile = await _context.AIGymProfiles
                .FirstOrDefaultAsync(p => p.MemberId == userId);

            if (profile == null)
            {
                // إذا لم يوجد، نرسله لصفحة الإنشاء
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }

        // 2. عرض صفحة الإنشاء
        public IActionResult Create()
        {
            return View();
        }

        // 3. معالجة البيانات وإرسالها للذكاء الاصطناعي
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AIGymProfile model)
        {
            // إزالة التحقق من Member و MemberId لأننا سنملؤها يدوياً
            ModelState.Remove("Member");
            ModelState.Remove("MemberId");
            ModelState.Remove("AIPrompt");
            ModelState.Remove("AIResult");

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.MemberId = userId;

                // --- محاكاة الذكاء الاصطناعي (AI Simulation) ---
                // ملاحظة: في المشروع الحقيقي، هنا نستخدم HttpClient للاتصال بـ OpenAI API

                string aiResponse = "";
                double bmi = model.Weight / ((model.Height / 100) * (model.Height / 100));

                if (model.Goal.Contains("عضلات") || model.Goal.Contains("Muscle"))
                {
                    aiResponse = $"بناءً على وزنك ({model.Weight}kg) وطولك، خطة الذكاء الاصطناعي تقترح: التركيز على تمارين المقاومة (Hypertrophy) 4 أيام أسبوعياً، مع زيادة البروتين إلى 2 جرام لكل كيلو من وزن الجسم. مؤشر كتلة جسمك هو {bmi:F1}.";
                }
                else if (model.Goal.Contains("وزن") || model.Goal.Contains("Weight"))
                {
                    aiResponse = $"لتحقيق هدفك في إنقاص الوزن، يقترح النظام: عجز في السعرات الحرارية بمقدار 500 سعرة يومياً، مع المشي لمدة 30 دقيقة بعد التمارين. مؤشر كتلة جسمك الحالي هو {bmi:F1}، وهو يحتاج لبعض التحسين.";
                }
                else
                {
                    aiResponse = $"نصيحة عامة للياقة: حافظ على نشاطك لمدة 150 دقيقة أسبوعياً. مؤشر كتلة جسمك هو {bmi:F1}، وهو في النطاق الصحي.";
                }

                model.AIPrompt = $"User Stats: Weight {model.Weight}, Height {model.Height}, Goal: {model.Goal}";
                model.AIResult = aiResponse;
                // -----------------------------------------------

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // زر لحذف الملف وإعادة المحاولة
        public async Task<IActionResult> Reset()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _context.AIGymProfiles.FirstOrDefaultAsync(p => p.MemberId == userId);

            if (profile != null)
            {
                _context.AIGymProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Create));
        }
    }
}