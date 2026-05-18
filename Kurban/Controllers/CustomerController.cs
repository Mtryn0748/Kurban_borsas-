using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Kurban.Models;

namespace Kurban.Controllers
{
    public class CustomerController : Controller
    {
        private readonly KurbanlikContext _context;

        public CustomerController(KurbanlikContext context)
        {
            _context = context;
        }




        // Müşteri Listesi
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }



        // Müşteri Ekleme Sayfası (Get)
        public IActionResult Create()
        {
            return View();
        }




        // Müşteri Ekleme İşlemi (Post)
        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}