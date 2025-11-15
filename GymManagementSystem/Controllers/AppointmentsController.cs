using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // 1. عرض صفحة الحجز
        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Book()
        {
            var classes = await _context.GroupClasses.AsNoTracking().ToListAsync();
            ViewBag.GroupClassId = new SelectList(classes, "GroupClassId", "Name");

            var model = new Appointment
            {
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            return View(model);
        }

        // 2. معالجة الحجز - النسخة المصلحة من خطأ MemberId
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            // أ. جلب معرف المستخدم الحالي وتعبئته يدوياً
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            appointment.MemberId = userId;
            appointment.IsConfirmed = false;

            // ب. حساب وقت الانتهاء بناءً على الحصة المختارة
            var groupClass = await _context.GroupClasses
                .Include(gc => gc.Appointments)
                .FirstOrDefaultAsync(gc => gc.GroupClassId == appointment.GroupClassId);

            if (groupClass != null)
            {
                appointment.EndTime = appointment.StartTime.AddMinutes(groupClass.DurationMinutes);

                // فحص السعة القصوى
                int confirmedCount = groupClass.Appointments.Count(a => a.IsConfirmed);
                if (confirmedCount >= groupClass.MaxCapacity)
                {
                    ModelState.AddModelError("", "Üzgünüz, bu dersin kontenjanı dolmuştur.");
                }
            }
            else
            {
                ModelState.AddModelError("GroupClassId", "Lütfen geçerli bir ders seçiniz.");
            }

            // ج. فحص تداخل المواعيد للمستخدم
            if (await CheckUserConflict(userId, appointment.StartTime, appointment.EndTime))
            {
                ModelState.AddModelError("StartTime", "Bu saat diliminde zaten onaylanmış بىر randevunuz bulunmaktadır.");
            }

            // د. [الحل النهائي للخطأ] حذف الحقول التي تسبب فشل التحقق
            // نقوم بإخبار النظام بتجاهل التحقق من هذه الحقول لأننا أدرناها برمجياً
            ModelState.Remove("MemberId");
            ModelState.Remove("Member");
            ModelState.Remove("Class");
            ModelState.Remove("EndTime");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Randevunuz alındı. Yönetici onayı bekleniyor.";
                    return RedirectToAction(nameof(MyAppointments));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Randevu kaydedilirken veritabanı hatası.");
                    ModelState.AddModelError("", "Teknik bir hata oluştu: " + ex.Message);
                }
            }

            // في حال فشل ModelState، نعيد تحميل القائمة المنسدلة
            var classes = await _context.GroupClasses.AsNoTracking().ToListAsync();
            ViewBag.GroupClassId = new SelectList(classes, "GroupClassId", "Name", appointment.GroupClassId);
            return View(appointment);
        }

        // 3. عرض حجوزات العضو
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _context.Appointments
                .Include(a => a.Class)
                .Where(a => a.MemberId == userId)
                .OrderByDescending(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync();

            return View(appointments);
        }

        // 4. لوحة تحكم الأدمن (الموافقات)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingApprovals()
        {
            var pending = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Class)
                .Where(a => !a.IsConfirmed)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            return View(pending);
        }

        // 5. أكشن الموافقة (للأدمن)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.IsConfirmed = true;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla onaylandı.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        // مساعد: فحص تداخل المواعيد
        private async Task<bool> CheckUserConflict(string userId, DateTime start, DateTime end)
        {
            return await _context.Appointments
                .AnyAsync(a => a.MemberId == userId &&
                               a.IsConfirmed &&
                               ((start < a.EndTime && end > a.StartTime)));
        }
    }
}