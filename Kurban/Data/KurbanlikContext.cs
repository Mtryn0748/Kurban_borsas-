using Kurban.Models;
using Microsoft.EntityFrameworkCore;


public class KurbanlikContext : DbContext
{
    // Programcs'den bağlantı cümlesini almak için Constructor
    public KurbanlikContext(DbContextOptions<KurbanlikContext> options) : base(options)
    {
    }

    // Veritabanında oluşacak tablolarımız (DbSet'ler)
    public DbSet<Breed> Breeds { get; set; } 
    public DbSet<Animal> Animals { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Owner> Owners { get; set; } 
    public DbSet<Sale> Sales { get; set; }
    public DbSet<User>Users { get; set; }
    public DbSet<Bid>Bids { get; set; }
}