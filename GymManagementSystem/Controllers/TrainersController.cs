using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

// تقييد الوصول: هذا الكنترولر متاح فقط للمدير (Admin)
[Authorize(Roles = "Admin")]
public class TrainersController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Trainers (Read All)
    public async Task<IActionResult> Index()
    {
        return View(await _context.Trainers.ToListAsync());
    }

    // GET: Trainers/Details/5 (Read One)
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trainer = await _context.Trainers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (trainer == null)
        {
            return NotFound();
        }

        return View(trainer);
    }

    // GET: Trainers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Trainers/Create (Create)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FullName,Specialty,ImageUrl")] Trainer trainer)
    {
        if (ModelState.IsValid)
        {
            _context.Add(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(trainer);
    }

    // GET: Trainers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trainer = await _context.Trainers.FindAsync(id);
        if (trainer == null)
        {
            return NotFound();
        }
        return View(trainer);
    }

    // POST: Trainers/Edit/5 (Update)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialty,ImageUrl")] Trainer trainer)
    {
        if (id != trainer.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(trainer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Trainers.Any(e => e.Id == trainer.Id))
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
        return View(trainer);
    }

    // GET: Trainers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trainer = await _context.Trainers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (trainer == null)
        {
            return NotFound();
        }

        return View(trainer);
    }

    // POST: Trainers/Delete/5 (Delete)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trainer = await _context.Trainers.FindAsync(id);
        if (trainer != null)
        {
            _context.Trainers.Remove(trainer);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}