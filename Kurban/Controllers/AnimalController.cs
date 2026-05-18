using Kurban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kurban.Controllers
{
    public class AnimalController : Controller
    {
        private readonly KurbanlikContext _context;



        public AnimalController(KurbanlikContext context)
        {
            _context = context;
        }





        public async Task<IActionResult> Index()
        {
            var animals = await _context.Animals.Include(a => a.Breed).ToListAsync();
            return View(animals);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Breeds = new SelectList(await _context.Breeds.ToListAsync(), "Id", "BreedName");
            return View();
        }





        [HttpPost]
        public async Task<IActionResult> Create(Animal animal)
        {
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var animal = await _context.Animals.Include(a => a.Breed).FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) return NotFound();
            return View(animal);
        }




        // Hayvan Düzenleme Sayfası (Get)
        // Hayvan Düzenleme Sayfası (Get)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            // CRITICAL SECURITY LOCK: Eğer hayvan satıldıysa düzenleme ekranına girişi ENGELLA!
            if (animal.IsSold)
            {
                TempData["ToastError"] = "Güvenlik İhlali: Satılmış bir hayvanın kafa kağıdı bilgileri değiştirilemez!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Breeds = new SelectList(await _context.Breeds.ToListAsync(), "Id", "BreedName");
            return View(animal);
        }






        // Hayvan Düzenleme İşlemi (Post)
        [HttpPost]
        public async Task<IActionResult> Edit(Animal animal)
        {
            // Backend Çift Katmanlı Koruma: Postman vb. araçlarla dışarıdan manipülasyonu önler
            var existingAnimal = await _context.Animals.AsNoTracking().FirstOrDefaultAsync(a => a.Id == animal.Id);
            if (existingAnimal != null && existingAnimal.IsSold)
            {
                TempData["ToastError"] = "Güvenlik İhlali: Satılan hayvan üzerinde değişiklik yapılamaz!";
                return RedirectToAction(nameof(Index));
            }

            _context.Animals.Update(animal);
            await _context.SaveChangesAsync();
            TempData["ToastSuccess"] = "Hayvan kafa kağıdı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }





        // YENİ EKLENEN ENVANTERDEN SİLME METODU (Post)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            // CRITICAL SECURITY LOCK: Satılan hayvan veritabanından SİLİNEMEZ! (Mali veri bütünlüğü koruması)
            if (animal.IsSold)
            {
                TempData["ToastError"] = "Kritik Hata: Satışı yapılmış ve sözleşmesi basılmış bir hayvan sistemden silinemez!";
                return RedirectToAction(nameof(Index));
            }

            // SQL Foreign Key Hatasını önlemek için: Eğer bu hayvana verilmiş ama bekleyen teklifler (Bids) varsa önce onları temizle
            var relatedBids = await _context.Bids.Where(b => b.AnimalId == id).ToListAsync();
            if (relatedBids.Any())
            {
                _context.Bids.RemoveRange(relatedBids);
            }

            // Hayvanı envanterden sök at
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            TempData["ToastSuccess"] = "Hayvan envanterden başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}