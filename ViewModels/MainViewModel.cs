using InventoryApp.Commands;
using InventoryApp.Models;
using InventoryApp.Services;
using InventoryApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InventoryApp.ViewModels
{
    /// <summary>
    /// Главная ViewModel. SOLID:
    ///   S — только координация UI ↔ Services
    ///   O — расширяется через добавление новых команд
    ///   D — зависит от абстракций (интерфейсы сервисов)
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IProductService       _productService;
        private readonly ICategoryService      _categoryService;
        private readonly IStockMovementService _movementService;

        // ══════════════════════════════════════════════════════════════
        // ObservableCollections — полные свойства (geттер → return _field)
        // ══════════════════════════════════════════════════════════════

        private ObservableCollection<Product> _products = new();
        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged("Products"); }
        }

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged("Categories"); }
        }

        private ObservableCollection<StockMovement> _movements = new();
        public ObservableCollection<StockMovement> Movements
        {
            get => _movements;
            set { _movements = value; OnPropertyChanged("Movements"); }
        }

        // ══════════════════════════════════════════════════════════════
        // Выбранные элементы
        // ══════════════════════════════════════════════════════════════

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
                // При выборе товара — показать его историю движений
                if (value != null)
                    _ = LoadMovementsAsync(value.Id);
            }
        }

        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged("SelectedCategory"); }
        }

        // ══════════════════════════════════════════════════════════════
        // Команды
        // ══════════════════════════════════════════════════════════════
        public ICommand AddProductCommand       { get; }
        public ICommand EditProductCommand      { get; }
        public ICommand DeleteProductCommand    { get; }
        public ICommand AddMovementCommand      { get; }
        public ICommand FilterByCategoryCommand { get; }
        public ICommand LoadAllCommand          { get; }

        // ══════════════════════════════════════════════════════════════
        // Конструктор
        // ══════════════════════════════════════════════════════════════
        public MainViewModel(
            IProductService       productService,
            ICategoryService      categoryService,
            IStockMovementService movementService)
        {
            _productService  = productService;
            _categoryService = categoryService;
            _movementService = movementService;

            AddProductCommand    = new RelayCommand(_ => _ = AddProductAsync());
            EditProductCommand   = new RelayCommand(_ => _ = EditProductAsync(),
                                                   _ => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(_ => _ = DeleteProductAsync(),
                                                   _ => SelectedProduct != null);
            AddMovementCommand   = new RelayCommand(_ => _ = AddMovementAsync());
            FilterByCategoryCommand = new RelayCommand(_ => _ = FilterByCategoryAsync(),
                                                      _ => SelectedCategory != null);
            LoadAllCommand = new RelayCommand(_ => _ = LoadAllAsync());

            // Загрузка при старте
            _ = LoadAllAsync();
        }

        // ══════════════════════════════════════════════════════════════
        // Методы загрузки данных
        // ══════════════════════════════════════════════════════════════

        public async Task LoadAllAsync()
        {
            try
            {
                var products   = await _productService.GetAllAsync();
                var categories = await _categoryService.GetAllAsync();
                var movements  = await _movementService.GetAllAsync();

                Products   = new ObservableCollection<Product>(products);
                Categories = new ObservableCollection<Category>(categories);
                Movements  = new ObservableCollection<StockMovement>(movements);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private async Task LoadMovementsAsync(int productId)
        {
            try
            {
                var list = await _movementService.GetByProductAsync(productId);
                Movements = new ObservableCollection<StockMovement>(list);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private async Task FilterByCategoryAsync()
        {
            if (SelectedCategory == null) return;
            try
            {
                var list = await _productService.GetByCategoryAsync(SelectedCategory.Id);
                Products = new ObservableCollection<Product>(list);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ══════════════════════════════════════════════════════════════
        // CRUD — Товары
        // ══════════════════════════════════════════════════════════════

        private async Task AddProductAsync()
        {
            var categories = await _categoryService.GetAllAsync();
            var dlg = new ProductDialog(categories)
            {
                Owner = Application.Current.MainWindow
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                var product = dlg.VM.ToProduct();
                await _productService.AddAsync(product);
                await LoadAllAsync();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private async Task EditProductAsync()
        {
            if (SelectedProduct == null) return;

            var categories = await _categoryService.GetAllAsync();
            var dlg = new ProductDialog(categories, SelectedProduct)
            {
                Owner = Application.Current.MainWindow
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                var product = dlg.VM.ToProduct();
                // Сохраняем остаток — редактирование не меняет количество
                product.Quantity = SelectedProduct.Quantity;
                await _productService.UpdateAsync(product);
                await LoadAllAsync();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                $"Удалить товар «{SelectedProduct.Name}»?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _productService.DeleteAsync(SelectedProduct.Id);
                await LoadAllAsync();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ══════════════════════════════════════════════════════════════
        // Движения товаров
        // ══════════════════════════════════════════════════════════════

        private async Task AddMovementAsync()
        {
            var products = await _productService.GetAllAsync();
            if (products.Count == 0)
            {
                MessageBox.Show("Сначала добавьте товары.", "Нет товаров",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new MovementDialog(products, SelectedProduct)
            {
                Owner = Application.Current.MainWindow
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                var movement = dlg.VM.ToMovement();
                await _movementService.RegisterMovementAsync(movement);
                await LoadAllAsync();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ══════════════════════════════════════════════════════════════
        // Helper
        // ══════════════════════════════════════════════════════════════

        private static void ShowError(Exception ex) =>
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
