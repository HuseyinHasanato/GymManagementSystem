using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;


// Sadece Yönetici rolüne sahip kullanıcılar CRUD işlemlerini yapabilir
[Authorize(Roles = "Admin")]
public class GroupClassesController : Controller
{
    private readonly ApplicationDbContext _context;

    public GroupClassesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /GroupClasses/Index (Ders Listeleme)
    // [AllowAnonymous] olduğundan, giriş yapmamış herkes dersleri görebilir (hizmet kataloğu)
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        // Eğitmen (Trainer) bilgisini dahil ederek tüm dersleri asenkron olarak getirir
        var applicationDbContext = _context.GroupClasses.Include(g => g.Trainer);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: /GroupClasses/Details/{id} (Ders Detayları)
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var groupClass = await _context.GroupClasses
            .Include(g => g.Trainer) // Eğitmen detaylarını getir
            .FirstOrDefaultAsync(m => m.GroupClassId == id);

        if (groupClass == null)
        {
            return NotFound(); // Ders bulunamadı hatası
        }

        return View(groupClass);
    }

    // GET: /GroupClasses/Create (Yeni Ders Oluşturma Formu)
    // Sadece Adminler erişebilir (Kontrolcü seviyesinde yetkilendirme var)
    public IActionResult Create()
    {
        // Eğitmen seçimi için SelectList oluşturma
        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
        return View();
    }

    // POST: /GroupClasses/Create (Yeni Dersi Kaydet)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("GroupClassId,Name,Description,StartTime,MaxCapacity,TrainerId,DurationMinutes")] GroupClass groupClass)
    {
        // Modelin geçerli olup olmadığını ve DurationMinutes'ın dahil edildiğini kontrol et
        if (ModelState.IsValid)
        {
            _context.Add(groupClass);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Yeni ders/hizmet başarıyla sisteme eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // Hata durumunda formu yeniden göster ve Eğitmen listesini tekrar doldur
        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

    // GET: /GroupClasses/Edit/{id} (Ders Düzenleme Formu)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var groupClass = await _context.GroupClasses.FindAsync(id);

        if (groupClass == null) return NotFound();

        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

    // POST: /GroupClasses/Edit/{id} (Ders Bilgilerini Güncelle)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("GroupClassId,Name,Description,StartTime,MaxCapacity,TrainerId,DurationMinutes")] GroupClass groupClass)
    {
        if (id != groupClass.GroupClassId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(groupClass);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ders bilgileri başarıyla güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.GroupClasses.Any(e => e.GroupClassId == groupClass.GroupClassId))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Eşzamanlılık hatası
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", groupClass.TrainerId);
        return View(groupClass);
    }

    // GET: /GroupClasses/Delete/{id} (Silme Onay Formu)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var groupClass = await _context.GroupClasses
            .Include(g => g.Trainer) // Eğitmen adını göstermek için dahil et
            .FirstOrDefaultAsync(m => m.GroupClassId == id);

        if (groupClass == null) return NotFound();
        return View(groupClass);
    }

    // POST: /GroupClasses/Delete/{id} (Silme İşlemini Onayla)
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
        TempData["SuccessMessage"] = "Ders başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }
}