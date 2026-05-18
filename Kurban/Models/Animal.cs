using System.ComponentModel.DataAnnotations.Schema;

namespace Kurban.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string AnimalEarringNumber { get; set; }
        public string? AnimalName { get; set; }

        // CRITICAL FIX: Gram hassasiyeti için decimal(18,3) yapıldı (Örn: 6.750 kg)
        [Column(TypeName = "decimal(18,3)")]
        public decimal AnimalWeight { get; set; }

        public string Age { get; set; }
        public string InventoryStatus { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerKg { get; set; }

        public int BreedId { get; set; }
        public Breed? Breed { get; set; }
        public bool IsSold { get; set; } = false;
    }
}