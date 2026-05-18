using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Kurban.Models;

namespace Kurban.Controllers
{
    public class SaleController : Controller
    {
        private readonly KurbanlikContext _context;

        public SaleController(KurbanlikContext context)
        {
            _context = context;
        }





        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Animal)
                .Include(s => s.Buyer)
                .ToListAsync();
            return View(sales);
        }





        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // CRITICAL FIX: Sadece satılmamış VE durumu "Satılık" olan hayvanlar listelenir!
            // Damızlıklar ve Yeni Doğanlar bu listede asla görünmez.
            var unsoldAnimals = await _context.Animals
                .Include(a => a.Breed)
                .Where(a => a.IsSold == false && a.InventoryStatus == "Satılık")
                .ToListAsync();

            var buyers = await _context.Users.Where(u => u.Role == "Buyer").ToListAsync();

            ViewBag.Animals = new SelectList(unsoldAnimals.Select(a => new
            {
                Id = a.Id,
                DisplayText = $"{a.AnimalEarringNumber} - {a.AnimalName} ({a.AnimalWeight} kg) [{a.Age}]"
            }), "Id", "DisplayText");

            ViewBag.Buyers = new SelectList(buyers.Select(b => new
            {
                Id = b.Id,
                FullName = $"{b.Name} {b.Surname}"
            }), "Id", "FullName");

            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Create(Sale sale)
        {
            var currentOwner = await _context.Owners.FirstOrDefaultAsync();
            if (currentOwner != null)
            {
                sale.OwnerId = currentOwner.Id;
            }

            _context.Sales.Add(sale);

            var animal = await _context.Animals.FindAsync(sale.AnimalId);
            if (animal != null)
            {
                animal.IsSold = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        // Gelen Teklifleri Listeleme (Admin Panel)
        // Gelen Teklifleri Listeleme (Admin Panel)
        public async Task<IActionResult> IncomingBids()
        {
            // Sadece hala pazarlığı devam eden aktif süreçleri listeliyoruz
            var bids = await _context.Bids
                .Include(b => b.Animal).ThenInclude(a => a.Breed)
                .Include(b => b.Buyer)
                .Where(b => b.Status == "Beklemede" || b.Status == "Karşı Teklif Yapıldı")
                .ToListAsync();
            return View(bids);
        }



        // Adminin Alıcının Teklifini Doğrudan Kabul Etmesi (Top Admin'deyken çalışır)
        [HttpPost]
        public async Task<IActionResult> AcceptBid(int bidId, decimal slaughterPrice, string deliveryType)
        {
            var bid = await _context.Bids.Include(b => b.Animal).FirstOrDefaultAsync(b => b.Id == bidId);
            var owner = await _context.Owners.FirstOrDefaultAsync();

            if (bid != null && bid.Animal != null)
            {
                // GÜVENLİK KİLİDİ: Top alıcıdaysa admin hile yapıp onaylayamaz
                if (bid.Status == "Karşı Teklif Yapıldı") return BadRequest("Müşteri yanıtı bekleniyor!");

                bid.Status = "Onaylandı";
                bid.Animal.IsSold = true;

                var newSale = new Sale
                {
                    AnimalId = bid.AnimalId,
                    BuyerId = bid.BuyerId,
                    SalePrice = bid.OfferPrice, // Alıcının en son sunduğu fiyat onaylandı
                    DeliveryType = deliveryType,
                    SlaughterPrice = slaughterPrice,
                    UsageType = "Kurbanlık",
                    Notes = $"Pazarlık Sözleşmesi. Müşteri Notu: {bid.BuyerNote}",
                    OwnerId = owner?.Id ?? 1
                };

                _context.Sales.Add(newSale);

                // Concurrency Koruması: Aynı hayvana gelen diğer bekleyen teklifleri otomatik REDDET
                var otherBids = await _context.Bids
                    .Where(b => b.AnimalId == bid.AnimalId && b.Id != bidId && b.Status == "Beklemede")
                    .ToListAsync();
                foreach (var other in otherBids) { other.Status = "Reddedildi (Başka Alıcıya Satıldı)"; }

                await _context.SaveChangesAsync();
                TempData["ToastSuccess"] = "Pazarlık başarıyla sonuçlandı, sözleşme imzalandı!";
            }
            return RedirectToAction(nameof(IncomingBids));
        }





        // Adminin Karşı Teklif Atması (Topu Alıcıya Fırlatır ve Admini Kilitleyen Nokta)
        [HttpPost]
        public async Task<IActionResult> CounterOffer(int bidId, decimal counterPrice)
        {
            var bid = await _context.Bids.FindAsync(bidId);
            if (bid != null && bid.Status == "Beklemede")
            {
                bid.CounterPrice = counterPrice;
                bid.Status = "Karşı Teklif Yapıldı"; // TOP ALICIYA GEÇTİ!
                await _context.SaveChangesAsync();
                TempData["ToastSuccess"] = "Karşı teklifiniz alıcıya fırlatıldı. Dönüş bekleniyor.";
            }
            return RedirectToAction(nameof(IncomingBids));
        }





        // Adminin Teklifi Doğrudan Reddedip Süreci Bitirmesi
        [HttpPost]
        public async Task<IActionResult> RejectBid(int bidId)
        {
            var bid = await _context.Bids.FindAsync(bidId);
            if (bid != null && bid.Status == "Beklemede")
            {
                bid.Status = "Reddedildi";
                await _context.SaveChangesAsync();
                TempData["ToastError"] = "Teklifi reddettiniz, pazarlık sonlandırıldı.";
            }
            return RedirectToAction(nameof(IncomingBids));
        }


        // Resmi Sözleşme Yazdırma Detayı
        public async Task<IActionResult> Details(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Animal).ThenInclude(a => a.Breed)
                .Include(s => s.Buyer)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();
            return View(sale);
        }



    }
}