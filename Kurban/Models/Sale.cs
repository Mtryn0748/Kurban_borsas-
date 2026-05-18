using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kurban.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public string UsageType { get; set; }
        public string DeliveryType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SlaughterPrice { get; set; }

        public DateTime PickupDate { get; set; } = DateTime.Now.AddDays(10);
        public string? Notes { get; set; }

        // Yabancı Anahtarlar (Sayısal Değerler Zorunlu Kalıyor)
        public int BuyerId { get; set; }
        public int AnimalId { get; set; }
        public int OwnerId { get; set; }

        // CRITICAL FIX: Soru işaretleri (?) eklenerek EF Core'un çökmesi engellendi
        public User? Buyer { get; set; }
        public Animal? Animal { get; set; }
        public Owner? Owner { get; set; }
    }
}