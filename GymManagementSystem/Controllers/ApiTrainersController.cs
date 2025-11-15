using GymManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;
using System.Linq; // مطلوب لـ LINQ

// استخدام سمة ApiController لإعدادات API التلقائية
[Route("api/[controller]")]
[ApiController]
public class ApiTrainersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ApiTrainersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ApiTrainers/Available?startTime=2025-12-20T10:00:00&endTime=2025-12-20T11:00:00
    // هذا يلبي متطلب: "Belirli bir tarihte uygun antrenörleri getirme" [cite: 27]
    [HttpGet("Available")]
    public async Task<ActionResult<IEnumerable<Trainer>>> GetAvailableTrainers(
        [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        if (startTime == default || endTime == default)
        {
            return BadRequest("يجب تحديد تاريخ ووقت البدء والانتهاء.");
        }

        // استخدام استعلام LINQ واحد لجلب جميع المدربين 
        // الذين ليس لديهم مواعيد تتداخل مع الفترة المطلوبة. (LINQ Filtering )
        var availableTrainers = await _context.Trainers
            .Where(t => !_context.Appointments
                .Include(a => a.Class)
                .Any(a =>
                    a.Class.TrainerId == t.Id && // الموعد مرتبط بالمدرب الحالي
                    (a.StartTime < endTime && a.EndTime > startTime))) // منطق التداخل
            .ToListAsync();

        if (availableTrainers == null || !availableTrainers.Any())
        {
            return NotFound("لم يتم العثور على مدربين متاحين في هذه الفترة.");
        }

        return Ok(availableTrainers);
    }
}