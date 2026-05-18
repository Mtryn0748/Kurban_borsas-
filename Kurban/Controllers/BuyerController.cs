using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Kurban.Models;

namespace Kurban.Controllers
{
    public class BuyerController : Controller
    {
        private readonly KurbanlikContext _context;

        public BuyerController(KurbanlikContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Home");
            int userId = int.Parse(userIdString);

            var myAnimals = await _context.Sales
                .Include(s => s.Animal).ThenInclude(a => a.Breed)
                .Where(s => s.BuyerId == userId)
                .ToListAsync();

            return View(myAnimals);
        }

        public async Task<IActionResult> Market()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Home");
            int userId = int.Parse(userIdString);

            var activeBidAnimalIds = await _context.Bids
                .Where(b => b.BuyerId == userId && (b.Status == "Beklemede" || b.Status == "Karşı Teklif Yapıldı"))
                .Select(b => b.AnimalId)
                .ToListAsync();

            var availableAnimals = await _context.Animals
                .Include(a => a.Breed)
                .Where(a => a.IsSold == false && a.InventoryStatus == "Satılık" && !activeBidAnimalIds.Contains(a.Id))
                .ToListAsync();

            return View(availableAnimals);
        }

        public async Task<IActionResult> MyBids()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Home");
            int userId = int.Parse(userIdString);

            var myBids = await _context.Bids
                .Include(b => b.Animal).ThenInclude(a => a.Breed)
                .Where(b => b.BuyerId == userId)
                .OrderByDescending(b => b.BidDate)
                .ToListAsync();

            return View(myBids);
        }

        [HttpPost]
        public async Task<IActionResult> SendBid(Bid bid)
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Home");
            int userId = int.Parse(userIdString);

            bid.BuyerId = userId;
            bid.BidDate = DateTime.Now;
            bid.Status = "Beklemede"; // İLK TOP ADMİNE ATILDI

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
            TempData["ToastSuccess"] = "Pazarlık teklifiniz çiftliğe başarıyla iletildi.";
            return RedirectToAction("MyBids");
        }

        // Alıcının Adminin Karşı Teklifini Kabul Etmesi (Satış Sözleşmesini Başlatır)
        [HttpPost]
        public async Task<IActionResult> AcceptCounterOffer(int bidId)
        {
            var bid = await _context.Bids.Include(b => b.Animal).FirstOrDefaultAsync(b => b.Id == bidId);
            var owner = await _context.Owners.FirstOrDefaultAsync();

            if (bid != null && bid.Animal != null && bid.CounterPrice.HasValue)
            {
                if (bid.Status != "Karşı Teklif Yapıldı") return BadRequest("Geçersiz işlem!");

                bid.Status = "Onaylandı";
                bid.Animal.IsSold = true;

                var newSale = new Sale
                {
                    AnimalId = bid.AnimalId,
                    BuyerId = bid.BuyerId,
                    SalePrice = bid.CounterPrice.Value, // Adminin ara-bul fiyatı nihai fiyat oldu
                    DeliveryType = "Tesiste Kesilecek",
                    SlaughterPrice = 0,
                    UsageType = "Kurbanlık",
                    Notes = $"Müşteri Çiftliğin Karşı Teklifini Onayladı. Müşteri Notu: {bid.BuyerNote}",
                    OwnerId = owner?.Id ?? 1
                };

                _context.Sales.Add(newSale);

                var otherBids = await _context.Bids
                    .Where(b => b.AnimalId == bid.AnimalId && b.Id != bidId && b.Status == "Beklemede")
                    .ToListAsync();
                foreach (var other in otherBids) { other.Status = "Reddedildi (Hayvan Satıldı)"; }

                await _context.SaveChangesAsync();
                TempData["ToastSuccess"] = "Tebrikler! Karşı teklifi kabul ettiniz ve kurbanlığı satın aldınız.";
            }
            return RedirectToAction(nameof(Index));
        }

        // LOJİK TAMİR: Alıcının Adminin Karşı Teklifini Reddedip Pazarlığı Kapatması
        [HttpPost]
        public async Task<IActionResult> RejectCounterOffer(int bidId)
        {
            var bid = await _context.Bids.FindAsync(bidId);
            if (bid != null && bid.Status == "Karşı Teklif Yapıldı")
            {
                bid.Status = "Reddedildi";
                await _context.SaveChangesAsync();
                TempData["ToastError"] = "Çiftliğin karşı teklifini reddettiniz, süreç kapandı.";
            }
            return RedirectToAction(nameof(MyBids));
        }

        // SONSUZ DÖNGÜ KALBİ: Alıcının Adminin Karşı Teklifini Beğenmeyip Tekrar Karşı Teklif Atması
        [HttpPost]
        public async Task<IActionResult> CounterBargain(int bidId, decimal newOfferPrice)
        {
            var bid = await _context.Bids.FindAsync(bidId);
            if (bid != null && bid.Status == "Karşı Teklif Yapıldı")
            {
                bid.OfferPrice = newOfferPrice; // Lucas yeni fiyatını yazdı (Örn: 35k)
                bid.CounterPrice = null; // Eski admin teklifi temizlendi
                bid.Status = "Beklemede"; // TOP TEKRAR ADMİNE FIRLATILDI!
                bid.BuyerNote = "Alıcı Yeni Karşı Teklif Sundu.";

                await _context.SaveChangesAsync();
                TempData["ToastSuccess"] = "Yeni fiyat teklifiniz admin paneline geri fırlatıldı!";
            }
            return RedirectToAction(nameof(MyBids));
        }

        [HttpPost]
        public async Task<IActionResult> CancelBid(int bidId)
        {
            var bid = await _context.Bids.FindAsync(bidId);
            if (bid != null && bid.Status == "Beklemede")
            {
                _context.Bids.Remove(bid);
                await _context.SaveChangesAsync();
                TempData["ToastError"] = "Teklifiniz başarıyla iptal edildi.";
            }
            return RedirectToAction(nameof(MyBids));
        }

        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Home");
            int userId = int.Parse(userIdString);
            var user = await _context.Users.FindAsync(userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User updatedUser)
        {
            var user = await _context.Users.FindAsync(updatedUser.Id);
            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Surname = updatedUser.Surname;
                user.Phone = updatedUser.Phone;
                user.Password = updatedUser.Password;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                TempData["ToastSuccess"] = "Profil bilgileriniz başarıyla güncellendi!";
            }
            return View(user);
        }
    }
}