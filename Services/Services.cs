using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.Services
{
    // ─── Product Service ────────────────────────────────────────────────
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        public ProductService(AppDbContext db) => _db = db;

        public async Task<List<Product>> GetAllAsync() =>
            await _db.Products.Include(p => p.Category).ToListAsync();

        public async Task<List<Product>> GetByCategoryAsync(int categoryId) =>
            await _db.Products
                     .Include(p => p.Category)
                     .Where(p => p.CategoryId == categoryId)
                     .ToListAsync();

        public async Task AddAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p != null)
            {
                _db.Products.Remove(p);
                await _db.SaveChangesAsync();
            }
        }
    }

    // ─── Category Service ───────────────────────────────────────────────
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;
        public CategoryService(AppDbContext db) => _db = db;

        public async Task<List<Category>> GetAllAsync() =>
            await _db.Categories.ToListAsync();

        public async Task AddAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _db.Categories.FindAsync(id);
            if (c != null) { _db.Categories.Remove(c); await _db.SaveChangesAsync(); }
        }
    }

    // ─── StockMovement Service ──────────────────────────────────────────
    /// <summary>
    /// Регистрирует приход / расход и атомарно обновляет остаток.
    /// SOLID: единственная ответственность — бизнес-логика движений.
    /// </summary>
    public class StockMovementService : IStockMovementService
    {
        private readonly AppDbContext _db;
        public StockMovementService(AppDbContext db) => _db = db;

        public async Task<List<StockMovement>> GetByProductAsync(int productId) =>
            await _db.StockMovements
                     .Include(m => m.Product)
                     .Where(m => m.ProductId == productId)
                     .OrderByDescending(m => m.Date)
                     .ToListAsync();

        public async Task<List<StockMovement>> GetAllAsync() =>
            await _db.StockMovements
                     .Include(m => m.Product)
                     .OrderByDescending(m => m.Date)
                     .ToListAsync();

        public async Task RegisterMovementAsync(StockMovement movement)
        {
            var product = await _db.Products.FindAsync(movement.ProductId)
                          ?? throw new InvalidOperationException("Товар не найден.");

            if (movement.Type == MovementType.Out && product.Quantity < movement.Quantity)
                throw new InvalidOperationException(
                    $"Недостаточно товара. Остаток: {product.Quantity}, запрошено: {movement.Quantity}.");

            // Атомарное обновление остатка
            product.Quantity += movement.Type == MovementType.In
                                 ? movement.Quantity
                                 : -movement.Quantity;

            movement.Date = DateTime.Now;
            _db.StockMovements.Add(movement);
            await _db.SaveChangesAsync();
        }
    }
}
