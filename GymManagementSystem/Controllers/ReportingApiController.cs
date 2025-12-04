using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;

namespace GymManagementSystem.Controllers
{
    // تعريف المسار: api/ReportingApi
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportingApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. تقرير: جلب جميع المواعيد النشطة (القادمة)
        // GET: api/ReportingApi/ActiveAppointments
        [HttpGet("ActiveAppointments")]
        public async Task<ActionResult<IEnumerable<object>>> GetActiveAppointments()
        {
            // استخدام LINQ لفلترة المواعيد التي لم يمر وقتها بعد
            var activeAppointments = await _context.Appointments
                .Include(a => a.Trainer)    // تضمين بيانات المدرب
                .Include(a => a.GymService) // تضمين بيانات الخدمة
                .Where(a => a.AppointmentDate > DateTime.Now) // الشرط: التاريخ أكبر من الآن
                .OrderBy(a => a.AppointmentDate) // ترتيب تصاعدي حسب الوقت
                .Select(a => new // اختيار حقول محددة لعرضها في JSON بشكل نظيف
                {
                    AppointmentId = a.Id,
                    Date = a.AppointmentDate,
                    ServiceName = a.GymService.Name,
                    TrainerName = a.Trainer.FullName,
                    Price = a.GymService.Price
                })
                .ToListAsync();

            if (activeAppointments == null || !activeAppointments.Any())
            {
                return NotFound("No active appointments found.");
            }

            return Ok(activeAppointments);
        }

        // 2. تقرير: جلب مواعيد مدرب معين
        // GET: api/ReportingApi/TrainerSchedule/5
        [HttpGet("TrainerSchedule/{trainerId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainerSchedule(int trainerId)
        {
            var schedule = await _context.Appointments
                .Include(a => a.GymService)
                .Where(a => a.TrainerId == trainerId) // فلترة حسب رقم المدرب
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new
                {
                    Date = a.AppointmentDate,
                    Service = a.GymService.Name,
                    Duration = a.GymService.Duration
                })
                .ToListAsync();

            if (schedule == null || !schedule.Any())
            {
                return NotFound($"No appointments found for trainer ID {trainerId}.");
            }

            return Ok(schedule);
        }

        // 3. تقرير: جلب جميع المدربين وتخصصاتهم (قائمة بسيطة)
        // GET: api/ReportingApi/AllTrainers
        [HttpGet("AllTrainers")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTrainers()
        {
            var trainers = await _context.Trainers
                .Select(t => new
                {
                    Id = t.Id,
                    Name = t.FullName,
                    Expertise = t.Expertise
                })
                .ToListAsync();

            return Ok(trainers);
        }
    }
}