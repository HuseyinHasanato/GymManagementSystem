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


                string aiResponse = "";
                double bmi = model.Weight / ((model.Height / 100) * (model.Height / 100));

                if (model.Goal.Contains("Muscle"))
                {
                    aiResponse = $"Kilonuza ({model.Weight}kg) ve boyunuza göre yapay zeka planı: Haftada 4 gün hipertrofi (kas büyütme) antrenmanına odaklanın. Günlük protein alımınızı vücut ağırlığınızın 2 katına (gram cinsinden) çıkarın. Vücut Kitle İndeksiniz (BMI): {bmi:F1}.";
                }
                else if (model.Goal.Contains("Lose Weight"))
                {
                    aiResponse = $"Kilo verme hedefiniz için sistem önerisi: Günlük 500 kalori açığı oluşturun ve antrenman sonrası 30 dakika kardiyo yapın. Şeker ve işlenmiş gıdalardan kaçının. Mevcut Vücut Kitle İndeksiniz: {bmi:F1}.";
                }
                else
                {
                    aiResponse = $"Genel sağlık ve fitlik için öneri: Haftada en az 150 dakika orta tempolu aktivite yapın. Uykunuza dikkat edin ve bol su için. Vücut Kitle İndeksiniz {bmi:F1} olup sağlıklı aralıktadır.";
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