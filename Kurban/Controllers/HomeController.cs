using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

using Kurban.Models;
using Microsoft.EntityFrameworkCore;

namespace Kurban.Controllers
{
    public class HomeController : Controller
    {
        private readonly KurbanlikContext _context;

        public HomeController(KurbanlikContext context)
        {
            _context = context;
        }




        // Ana Sayfa (Dashboard)
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login");

            ViewBag.TotalAnimals = await _context.Animals.CountAsync();
            ViewBag.SoldAnimals = await _context.Animals.CountAsync(a => a.IsSold == true);
            ViewBag.UnsoldAnimals = await _context.Animals.CountAsync(a => a.IsSold == false);

            decimal totalRevenue = await _context.Sales.SumAsync(s => s.SalePrice + s.SlaughterPrice);
            ViewBag.TotalRevenue = totalRevenue;

            // 5. MADDE: GRAFİK İÇİN TÜR DAĞILIMI SORGUSU
            var breedData = await _context.Animals
                .Include(a => a.Breed)
                .GroupBy(a => a.Breed.BreedName)
                .Select(g => new { BreedName = g.Key ?? "Belirtilmemiş", Count = g.Count() })
                .ToListAsync();

            // JavaScript'in anlayacağı basit array formatına dönüştürüyoruz
            ViewBag.ChartLabels = breedData.Select(b => b.BreedName).ToArray();
            ViewBag.ChartData = breedData.Select(b => b.Count).ToArray();

            return View();
        }



        // Giriş Sayfası (Görünüm)
        [HttpGet]
        public IActionResult Login()
        {
            // Eğer kullanıcı zaten giriş yapmışsa direkt ana sayfaya yolla
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }



        // Giriş İşlemi (Post)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Veritabanında bu email ve şifreye ait kullanıcı var mı?
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Kullanıcıya bir dijital kimlik kartı hazırlıyoruz (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name + " " + user.Surname),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role), // Rolü buraya gömüyoruz (Admin, Seller, Buyer)
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Tarayıcıya çerezi (Cookie) bırakıyoruz
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Senaryoya göre Role tabanlı yönlendirme yapıyoruz
                // HomeController.cs -> Login Post metodunun içi
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home"); // Admin -> Admin Dashboard'a
                }
                else if (user.Role == "Buyer")
                {
                    return RedirectToAction("Index", "Buyer"); // FIX: Alıcı Mehmet direkt kendi sayfasına gidecek!
                }
            }

            // Hatalı giriş durumunda sayfaya mesaj yollayalım
            ViewBag.Error = "Hatalı E-Posta veya Şifre!";
            return View();
        }




        // Çıkış İşlemi
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }



        // kayıt ekranı ekliyoruz 

        // Kayıt Ol Sayfası (Get)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt Ol İşlemi (Post)
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // Dışarıdan kayıt olan herkes varsayılan olarak Alıcı (Buyer) rolündedir
            user.Role = "Buyer";

            // Email adresi sistemde daha önce kullanılmış mı kontrolü
            var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (exists)
            {
                ViewBag.Error = "Bu e-posta adresi zaten sisteme kayıtlı!";
                return View(user);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Kayıt başarılı olunca kullanıcıyı doğrudan giriş ekranına yolluyoruz
            return RedirectToAction("Login");
        }
    }
}