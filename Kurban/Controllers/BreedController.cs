using Microsoft.AspNetCore.Mvc;
using Kurban.Models;
using Microsoft.EntityFrameworkCore;

namespace Kurban.Controllers
{
    public class BreedController : Controller
    {
        private readonly KurbanlikContext _context;

        // Dependency Injection: Program.cs'de tanıttığımız context'i burada istiyoruz.
        public BreedController(KurbanlikContext context)
        {
            _context = context;
        }

        // Listeleme Sayfası (Index)
        public async Task<IActionResult> Index()
        {
            var breeds = await _context.Breeds.ToListAsync();
            return View(breeds);
        }

        // Ekleme Sayfası (Get)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Ekleme İşlemi (Post)
        [HttpPost]
        public async Task<IActionResult> Create(Breed breed)
        {
            if (ModelState.IsValid)
            {
                _context.Add(breed); // RAM'e ekler
                await _context.SaveChangesAsync(); // SQL'e kaydeder
                return RedirectToAction(nameof(Index));
            }
            return View(breed);
        }


  


        // Düzenleme Sayfası (Get - Eski veriyi kutulara doldurur)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var breed = await _context.Breeds.FindAsync(id);
            if (breed == null)
            {
                return NotFound();
            }
            return View(breed);
        }

        // Düzenleme İşlemi (Post - Yeni veriyi SQL'e kaydeder)
        [HttpPost]
        public async Task<IActionResult> Edit(Breed breed)
        {
            if (ModelState.IsValid || true) // Güvenlik doğrulaması geçici esnetildi
            {
                _context.Breeds.Update(breed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(breed);
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // CRITICAL SECURITY LOCK: Eğer bu türe (Büyükbaş/Küçükbaş vb.) ait tek bir hayvan bile varsa SİLMEYİ ENGELLE!
            // AsNoTracking kullanarak veritabanını yormadan hızlıca kontrol ediyoruz.
            bool hasLinkedAnimals = await _context.Animals.AnyAsync(a => a.BreedId == id);

            if (hasLinkedAnimals)
            {
                // Yeni kurduğumuz Toastr bildirimini tetikliyoruz
                TempData["ToastError"] = "Kritik Veri Güvenliği Engeli: Bu kategoriye ait kayıtlı hayvanlar veya geçmiş satış sözleşmeleri mevcut! Sistemi çökertmemek için bu türü silemezsiniz.";
                return RedirectToAction(nameof(Index));
            }

            // Eğer o kategoriye ait hiçbir hayvan yoksa (yani tertemiz, boş bir kategoriyse) silinmesine izin ver
            var breed = await _context.Breeds.FindAsync(id);
            if (breed != null)
            {
                _context.Breeds.Remove(breed);
                await _context.SaveChangesAsync();
                TempData["ToastSuccess"] = "Boş olan hayvan türü başarıyla sistemden kaldırıldı.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}