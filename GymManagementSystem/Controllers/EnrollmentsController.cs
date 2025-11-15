using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


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

    
    public async Task<IActionResult> Index()
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        var enrollments = await _context.ClassEnrollments
            .Include(ce => ce.GroupClass)
            .ThenInclude(gc => gc!.Trainer) 
            .Where(ce => ce.UserId == userId)
            .ToListAsync();

        return View(enrollments);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int classId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        bool alreadyEnrolled = await _context.ClassEnrollments
            .AnyAsync(ce => ce.GroupClassId == classId && ce.UserId == userId);

        if (alreadyEnrolled)
        {
            TempData["ErrorMessage"] = "Bu derse zaten kaydoldunuz.";
            return RedirectToAction("Index", "GroupClasses"); 
        }

       
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

        
        var enrollment = new ClassEnrollment
        {
            GroupClassId = classId,
            UserId = userId
            
        };

        _context.ClassEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Derse başarıyla kaydoldunuz!";
        return RedirectToAction("Index", "GroupClasses");
    }

    
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
            return RedirectToAction(nameof(Index)); 
        }

        _context.ClassEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Kaydınız başarıyla iptal edildi.";
        return RedirectToAction(nameof(Index));
    }
}