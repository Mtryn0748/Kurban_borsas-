using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kurban.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public DateTime BidDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OfferPrice { get; set; } // Alıcının verdiği fiyat teklifi

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CounterPrice { get; set; } // Adminin verdiği karşı teklif (Opsiyonel)

        // Teklif Durumları: "Beklemede", "Onaylandı", "Reddedildi", "Karşı Teklif Yapıldı"
        public string Status { get; set; } = "Beklemede";
        public string? BuyerNote { get; set; } // Alıcının pazarlık mesajı

        // İlişkiler
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }

        public int BuyerId { get; set; }
        public User? Buyer { get; set; }
    }
}