using InventoryApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryApp.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetByCategoryAsync(int categoryId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }

    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task AddAsync(Category category);
        Task DeleteAsync(int id);
    }

    public interface IStockMovementService
    {
        Task<List<StockMovement>> GetByProductAsync(int productId);
        Task<List<StockMovement>> GetAllAsync();
        Task RegisterMovementAsync(StockMovement movement);
    }
}
