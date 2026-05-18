namespace Kurban.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Ödev projesi olduğu için şimdilik düz metin tutabiliriz
        public string Phone { get; set; }
        public string Role { get; set; } // "Admin", "Seller" veya "Buyer" değerlerini alacak
    }
}