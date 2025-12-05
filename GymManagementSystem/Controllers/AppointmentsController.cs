using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using System.Security.Claims; // لإحضار هوية المستخدم
using Microsoft.AspNetCore.Authorization; // لحماية الكنترولر

namespace GymManagementSystem.Controllers
{
    [Authorize] // تأكد أن المستخدم مسجل للدخول
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments (عرض المواعيد للعضو فقط، وللأدمن كل المواعيد)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointments = _context.Appointments
                .Include(a => a.GymService)
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .AsQueryable();

            // تحقق من الدور: إذا لم يكن أدمن، يفلتر حسب هويته فقط
            if (!User.IsInRole("Admin"))
            {
                appointments = appointments.Where(a => a.MemberId == userId);
            }

            return View(await appointments.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.GymService)
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["GymServiceId"] = new SelectList(_context.GymServices, "Id", "Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
            // حذف ViewData["MemberId"] لأنه يتم تعيينه تلقائيًا من السيرفر
            return View();
        }

        // POST: Appointments/Create (إضافة المنطق الذكي هنا)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // حذف [Bind] لضمان عدم تجاهل الخصائص التي يتم تعيينها يدوياً (مثل MemberId)
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // 1. تعيين هوية العضو تلقائياً (الحل لمشكلة NULL MemberId)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            appointment.MemberId = userId;

            // 2. التحقق من التعارض (Conflict Check)
            bool isBusy = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == appointment.TrainerId &&
                a.AppointmentDate == appointment.AppointmentDate);

            if (isBusy)
            {
                // رسالة الخطأ باللغة التركية
                ModelState.AddModelError("", "Bu saatte eğitmen meşgul, lütfen başka bir saat seçin.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync(); // هنا كان يحدث الخطأ
                return RedirectToAction(nameof(Index));
            }

            // إعادة تعبئة القوائم في حال وجود خطأ
            ViewData["GymServiceId"] = new SelectList(_context.GymServices, "Id", "Name", appointment.GymServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", appointment.TrainerId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["GymServiceId"] = new SelectList(_context.GymServices, "Id", "Name", appointment.GymServiceId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", appointment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", appointment.TrainerId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // يجب أن نستخدم [Bind] هنا لضمان عدم تغيير MemberId بشكل غير مقصود من الفورم
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppointmentDate,IsConfirmed,MemberId,GymServiceId,TrainerId")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            // يجب تعيين MemberId لضمان عدم إزالته من النموذج عند التعديل
            var originalAppointment = await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (originalAppointment == null) return NotFound();

            // التأكد من الحفاظ على MemberId الأصلي
            appointment.MemberId = originalAppointment.MemberId;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymServiceId"] = new SelectList(_context.GymServices, "Id", "Name", appointment.GymServiceId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", appointment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", appointment.TrainerId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.GymService)
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}