using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

// هذا الكنترولر متاح للأعضاء (Uye) فقط لإجراء الحجز
[Authorize(Roles = "Member")]
public class EnrollmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public EnrollmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Enrollments (عرض حجوزات العضو الحالية)
    public async Task<IActionResult> Index()
    {
        // 1. الحصول على UserId للمستخدم الحالي المسجل دخوله
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 2. جلب جميع حجوزات هذا المستخدم مع بيانات الحصة والمدرب
        var enrollments = await _context.ClassEnrollments
            .Include(ce => ce.GroupClass)
            .ThenInclude(gc => gc!.Trainer) // جلب المدرب المرتبط بالحصة
            .Where(ce => ce.UserId == userId)
            .ToListAsync();

        return View(enrollments);
    }

    // POST: Enrollments/Enroll (لإجراء الحجز)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int classId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. التحقق من وجود حجز سابق
        bool alreadyEnrolled = await _context.ClassEnrollments
            .AnyAsync(ce => ce.GroupClassId == classId && ce.UserId == userId);

        if (alreadyEnrolled)
        {
            TempData["ErrorMessage"] = "Bu derse zaten kaydoldunuz.";
            return RedirectToAction("Index", "GroupClasses"); // العودة لجدول الحصص
        }

        // 2. التحقق من السعة القصوى
        var groupClass = await _context.GroupClasses.FindAsync(classId);
        if (groupClass == null)
        {
            TempData["ErrorMessage"] = "Ders bulunamadı.";
            return RedirectToAction("Index", "GroupClasses");
        }

        var currentEnrollmentCount = await _context.ClassEnrollments.CountAsync(ce => ce.GroupClassId == classId);

        if (currentEnrollmentCount >= groupClass.MaxCapacity)
        {
            TempData["ErrorMessage"] = "Üzgünüm, bu dersin kapasitesi doludur.";
            return RedirectToAction("Index", "GroupClasses");
        }

        // 3. إنشاء الحجز
        var enrollment = new ClassEnrollment
        {
            GroupClassId = classId,
            UserId = userId
            // (يمكنك إضافة EnrollmentDate إذا كان موجوداً في الموديل)
        };

        _context.ClassEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Derse başarıyla kaydoldunuz!";
        return RedirectToAction("Index", "GroupClasses");
    }

    // POST: Enrollments/Cancel (لإلغاء الحجز)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int classId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var enrollment = await _context.ClassEnrollments
            .FirstOrDefaultAsync(ce => ce.GroupClassId == classId && ce.UserId == userId);

        if (enrollment == null)
        {
            TempData["ErrorMessage"] = "İptal edilecek bir kaydınız bulunamadı.";
            return RedirectToAction(nameof(Index)); // العودة لصفحة حجوزاتي
        }

        _context.ClassEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Kaydınız başarıyla iptal edildi.";
        return RedirectToAction(nameof(Index));
    }
}