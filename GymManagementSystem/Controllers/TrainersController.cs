using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;


[Authorize(Roles = "Admin")] // Sadece yöneticilerin erişimine izin ver
public class TrainersController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Trainers/Index (Tüm Eğitmenleri Listele)
    public async Task<IActionResult> Index()
    {
        // Eğitmen listesini veritabanından asenkron olarak alır
        return View(await _context.Trainers.ToListAsync());
    }

    // GET: /Trainers/Details/{id} (Eğitmen Detayları)
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound(); // Hata: ID belirtilmedi
        }

        var trainer = await _context.Trainers
            .FirstOrDefaultAsync(m => m.Id == id);

        if (trainer == null)
        {
            return NotFound(); // Hata: Eğitmen bulunamadı
        }

        return View(trainer);
    }

    // GET: /Trainers/Create (Eğitmen Oluşturma Formu)
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Trainers/Create (Yeni Eğitmeni Kaydet)
    [HttpPost]
    [ValidateAntiForgeryToken] // Güvenlik tokeni kontrolü
    public async Task<IActionResult> Create([Bind("Id,FullName,Specialty,ImageUrl")] Trainer trainer)
    {
        if (ModelState.IsValid)
        {
            _context.Add(trainer); // Eğitmen nesnesini ekle
            await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
            TempData["SuccessMessage"] = "Yeni eğitmen başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        // Doğrulama hatası varsa aynı formu geri göster
        return View(trainer);
    }

    // GET: /Trainers/Edit/{id} (Eğitmen Düzenleme Formu)
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

    // POST: /Trainers/Edit/{id} (Değişiklikleri Kaydet)
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
                _context.Update(trainer); // Eğitmen bilgilerini güncelle
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Eğitmen bilgileri başarıyla güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Trainers.Any(e => e.Id == trainer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Eşzamanlılık hatası (Concurrency)
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(trainer);
    }

    // GET: /Trainers/Delete/{id} (Silme Onay Formu)
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

    // POST: /Trainers/Delete/{id} (Silme İşlemini Onayla)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trainer = await _context.Trainers.FindAsync(id);
        if (trainer != null)
        {
            _context.Trainers.Remove(trainer); // Eğitmeni kaldır
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Eğitmen kaydı başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }
}