using InventoryApp.Data;
using InventoryApp.Services;
using InventoryApp.ViewModels;
using InventoryApp.Views;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace InventoryApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализация БД + автомиграция
            using var db = new AppDbContext();
            db.Database.Migrate();

            // Ручная «DI-сборка» (без контейнера — KISS)
            var context         = new AppDbContext();
            var productService  = new ProductService(context);
            var categoryService = new CategoryService(context);
            var movementService = new StockMovementService(context);

            var vm = new MainViewModel(productService, categoryService, movementService);

            var window = new MainWindow { DataContext = vm };
            window.Show();
        }
    }
}
