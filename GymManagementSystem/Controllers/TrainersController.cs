using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;


[Authorize(Roles = "Admin")]
public class TrainersController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainersController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task<IActionResult> Index()
    {
        return View(await _context.Trainers.ToListAsync());
    }

    
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

    
    public IActionResult Create()
    {
        return View();
    }

    
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