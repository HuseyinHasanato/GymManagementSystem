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
    [Authorize] // Tüm aksiyonlar için oturum açmış kullanıcı gereklidir
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Appointments/Book
        [HttpGet]
        [Authorize(Roles = "Member")] // Sadece üyeler randevu alabilir
        public async Task<IActionResult> Book()
        {
            // Dropdown listesi için Grup Derslerini getir
            ViewBag.GroupClassId = new SelectList(
                await _context.GroupClasses.ToListAsync(),
                "GroupClassId", "Name");

            var model = new Appointment
            {
                // Randevu başlangıç zamanını varsayılan olarak yarına 10:00 olarak ayarla
                StartTime = DateTime.Today.AddDays(1).AddHours(10),
                EndTime = DateTime.Today.AddDays(1).AddHours(11) // Varsayılan süre 1 saat
            };

            return View(model);
        }

        // POST: /Appointments/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            // 1. Üye ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            appointment.MemberId = userId;

            // Onay durumunu varsayılan olarak FALSE (Beklemede) ayarla
            appointment.IsConfirmed = false;

            // 2. Grup Dersinin süresini al ve EndTime'ı hesapla
            var groupClass = await _context.GroupClasses.FindAsync(appointment.GroupClassId);
            if (groupClass == null)
            {
                ModelState.AddModelError("", "Seçilen ders bulunamadı.");
                return View(appointment);
            }

            // EndTime hesaplama: Başlangıç zamanı + Ders süresi (dakika)
            appointment.EndTime = appointment.StartTime.AddMinutes(groupClass.DurationMinutes);

            // 3. Müsaitlik Kontrolü (Randevu çakışması kontrolü)
            var isConflict = await CheckAppointmentConflict(appointment.StartTime, appointment.EndTime);

            if (isConflict)
            {
                ModelState.AddModelError("StartTime", "Seçilen tarihte zaten onaylanmış bir randevu çakışması var. Lütfen farklı bir zaman dilimi seçin.");
            }

            // 4. Model Doğrulaması (Validation)
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevunuz başarıyla kaydedildi ve yönetici onayı bekleniyor.";
                return RedirectToAction(nameof(MyAppointments));
            }

            // Hata durumunda View'a geri dön
            ViewBag.GroupClassId = new SelectList(
                await _context.GroupClasses.ToListAsync(),
                "GroupClassId", "Name", appointment.GroupClassId);

            return View(appointment);
        }

        // GET: /Appointments/MyAppointments
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointments = await _context.Appointments
                .Include(a => a.Class)
                .Where(a => a.MemberId == userId)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();

            return View(appointments);
        }

        // GET: /Appointments/PendingApprovals
        [Authorize(Roles = "Admin")] // Sadece yöneticiler onay bekleyenleri görebilir
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingAppointments = await _context.Appointments
                .Include(a => a.Member) // Üye bilgilerini getir
                .Include(a => a.Class)  // Sınıf bilgilerini getir
                .Where(a => a.IsConfirmed == false) // Sadece onaylanmamışları filtrele
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            return View(pendingAppointments);
        }

        // POST: /Appointments/Approve/{id}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.IsConfirmed = true; // Onayla
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla onaylandı!";
            return RedirectToAction(nameof(PendingApprovals));
        }

        // POST: /Appointments/Reject/{id} (veya Cancel)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment); // Randevuyu sil (Reddetme)
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla reddedildi/iptal edildi.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        // Özel Yardımcı Metot: Müsaitlik Kontrolü
        private async Task<bool> CheckAppointmentConflict(DateTime newStartTime, DateTime newEndTime)
        {
            // Yalnızca aynı zaman dilimine çakışan onaylanmış (IsConfirmed == true) randevuları kontrol et
            var conflicts = await _context.Appointments
                .Where(a => a.IsConfirmed &&
                            (
                                (newStartTime < a.EndTime && newEndTime > a.StartTime) // Randevu aralığında çakışma
                            ))
                .AnyAsync();

            return conflicts;
        }
    }
}