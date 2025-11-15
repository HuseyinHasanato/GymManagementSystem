using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;


[Authorize(Roles = "Admin")]
public class GroupClassesController : Controller
{
    private readonly ApplicationDbContext _context;

    public GroupClassesController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        
        var applicationDbContext = _context.GroupClasses.Include(g => g.Trainer);
        return View(await applicationDbContext.ToListAsync());
    }

    
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var groupClass = await _context.GroupClasses
            .Include(g => g.Trainer)
            .FirstOrDefaultAsync(m => m.GroupClassId == id);

        if (groupClass == null)
        {
            return NotFound();
        }

        return View(groupClass);
    }

    
    public IActionResult Create()
    {
      
        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("GroupClassId,Name,Description,StartTime,MaxCapacity,TrainerId")] GroupClass groupClass)
    {
        
        if (ModelState.IsValid)
        {
            _context.Add(groupClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

   
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var groupClass = await _context.GroupClasses.FindAsync(id);
        if (groupClass == null) return NotFound();

        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

   
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

   
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var groupClass = await _context.GroupClasses
            .Include(g => g.Trainer) 
            .FirstOrDefaultAsync(m => m.GroupClassId == id);

        if (groupClass == null) return NotFound();
        return View(groupClass);
    }

    
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