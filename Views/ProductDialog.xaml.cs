using InventoryApp.Models;
using InventoryApp.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace InventoryApp.Views
{
    public partial class ProductDialog : Window
    {
        public ProductDialogViewModel VM { get; }

        public ProductDialog(List<Category> categories, Product? product = null)
        {
            InitializeComponent();
            VM = new ProductDialogViewModel(categories, product);
            DataContext = VM;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.IsValid)
            {
                MessageBox.Show("Исправьте ошибки перед сохранением.", "Валидация",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;
    }

    // ─── ViewModel диалога (IDataErrorInfo) ────────────────────────────
    public class ProductDialogViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public List<Category> Categories { get; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _sku = string.Empty;
        public string SKU
        {
            get => _sku;
            set { _sku = value; OnPropertyChanged(nameof(SKU)); }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }

        private int _categoryId;
        public int CategoryId
        {
            get => _categoryId;
            set { _categoryId = value; OnPropertyChanged(nameof(CategoryId)); }
        }

        public int? EditingId { get; private set; }

        public ProductDialogViewModel(List<Category> categories, Product? product)
        {
            Categories = categories;
            if (product != null)
            {
                EditingId  = product.Id;
                Name       = product.Name;
                SKU        = product.SKU;
                Quantity   = product.Quantity;
                CategoryId = product.CategoryId;
            }
        }

        // ── IDataErrorInfo ─────────────────────────────────────────────
        public string this[string columnName] => columnName switch
        {
            nameof(Name)       => string.IsNullOrWhiteSpace(Name) ? "Обязательное поле." : string.Empty,
            nameof(SKU)        => string.IsNullOrWhiteSpace(SKU)  ? "Обязательное поле." : string.Empty,
            nameof(Quantity)   => Quantity < 0 ? "Не может быть отрицательным." : string.Empty,
            nameof(CategoryId) => CategoryId <= 0 ? "Выберите категорию." : string.Empty,
            _ => string.Empty
        };

        public string Error =>
            this[nameof(Name)] + this[nameof(SKU)] +
            this[nameof(Quantity)] + this[nameof(CategoryId)];

        public bool IsValid => string.IsNullOrEmpty(Error);

        public Product ToProduct() => new()
        {
            Id         = EditingId ?? 0,
            Name       = Name,
            SKU        = SKU,
            Quantity   = Quantity,
            CategoryId = CategoryId
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
