using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=inventory.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SKU — unique index
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Электроника" },
                new Category { Id = 2, Name = "Канцелярия" },
                new Category { Id = 3, Name = "Прочее" }
            );
        }
    }
}
