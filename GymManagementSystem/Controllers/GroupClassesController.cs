using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

// تقييد الوصول: هذا الكنترولر متاح فقط للمدير (Admin)
[Authorize(Roles = "Admin")]
public class GroupClassesController : Controller
{
    private readonly ApplicationDbContext _context;

    public GroupClassesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: GroupClasses (Read All)
    // يسمح بالعرض للجميع لتحقيق متطلب "جدول الحصص"
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        // استخدام Include لجلب بيانات المدرب (Trainer) المرتبطة بالحصة
        var applicationDbContext = _context.GroupClasses.Include(g => g.Trainer);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: GroupClasses/Details/5 (Read One)
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var groupClass = await _context.GroupClasses
            .Include(g => g.Trainer) // جلب المدرب
            .FirstOrDefaultAsync(m => m.GroupClassId == id);

        if (groupClass == null)
        {
            return NotFound();
        }

        return View(groupClass);
    }

    // GET: GroupClasses/Create
    public IActionResult Create()
    {
        // تمرير قائمة المدربين إلى الـ View للسماح باختيار TrainerId
        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
        return View();
    }

    // POST: GroupClasses/Create (Create)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("GroupClassId,Name,Description,StartTime,MaxCapacity,TrainerId")] GroupClass groupClass)
    {
        // ملاحظة: يجب أن يكون ModelState.IsValid صحيحاً بعد إزالة حقل التنقل (Trainer) من Bind
        // لكننا نعيد تعبئة ViewData في حال فشل Validation لأي سبب آخر
        if (ModelState.IsValid)
        {
            _context.Add(groupClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

    // GET: GroupClasses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var groupClass = await _context.GroupClasses.FindAsync(id);
        if (groupClass == null) return NotFound();

        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

    // POST: GroupClasses/Edit/5 (Update)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("GroupClassId,Name,Description,StartTime,MaxCapacity,TrainerId")] GroupClass groupClass)
    {
        if (id != groupClass.GroupClassId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(groupClass);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.GroupClasses.Any(e => e.GroupClassId == groupClass.GroupClassId))
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
        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

    // GET: GroupClasses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var groupClass = await _context.GroupClasses
            .Include(g => g.Trainer) // جلب المدرب للعرض في صفحة التأكيد
            .FirstOrDefaultAsync(m => m.GroupClassId == id);

        if (groupClass == null) return NotFound();
        return View(groupClass);
    }

    // POST: GroupClasses/Delete/5 (Delete)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var groupClass = await _context.GroupClasses.FindAsync(id);
        if (groupClass != null)
        {
            _context.GroupClasses.Remove(groupClass);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}