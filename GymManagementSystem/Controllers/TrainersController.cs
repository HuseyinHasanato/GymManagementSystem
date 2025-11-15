using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TrainersController> _logger;

        public TrainersController(ApplicationDbContext context, ILogger<TrainersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. عرض جميع المدربين (مع تحسين الأداء)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers.AsNoTracking().ToListAsync();
            return View(trainers);
        }

        // 2. عرض صفحة إضافة مدرب جديد
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. معالجة إضافة مدرب جديد (حماية Bind)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialty,Bio,ImageUrl")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(trainer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Eğitmen başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Eğitmen eklenirken hata oluştu.");
                    ModelState.AddModelError("", "Sistem hatası: Kayıt yapılamadı.");
                }
            }
            return View(trainer);
        }

        // 4. عرض صفحة تعديل المدرب
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // 5. معالجة تعديل بيانات المدرب
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialty,Bio,ImageUrl")] Trainer trainer)
        {
            if (id != trainer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Eğitmen bilgileri güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id)) return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Eğitmen güncellenirken hata.");
                    ModelState.AddModelError("", "Güncelleme başarısız oldu.");
                }
            }
            return View(trainer);
        }

        // 6. حذف مدرب (مع معالجة القيود)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.GroupClasses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null) return NotFound();

            // التحقق مما إذا كان المدرب مرتبطاً بحصص قبل الحذف
            if (trainer.GroupClasses != null && trainer.GroupClasses.Any())
            {
                TempData["ErrorMessage"] = "Bu eğitmen silinemez çünkü atanmış dersleri bulunmaktadır.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Eğitmen başarıyla silindi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eğitmen silme hatası.");
                TempData["ErrorMessage"] = "Silme işlemi sırasında teknik bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}