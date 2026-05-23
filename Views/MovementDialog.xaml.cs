using InventoryApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace InventoryApp.Views
{
    public partial class MovementDialog : Window
    {
        public MovementDialogViewModel VM { get; }

        public MovementDialog(List<Product> products, Product? preselected = null)
        {
            InitializeComponent();
            VM = new MovementDialogViewModel(products, preselected);
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

    // ─── ViewModel диалога движения ────────────────────────────────────
    public class MovementDialogViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public List<Product>      Products      { get; }
        public List<MovementType> MovementTypes { get; } =
            new() { MovementType.In, MovementType.Out };

        private int _productId;
        public int ProductId
        {
            get => _productId;
            set { _productId = value; OnProp(nameof(ProductId)); }
        }

        private MovementType _selectedType = MovementType.In;
        public MovementType SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnProp(nameof(SelectedType)); }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnProp(nameof(Quantity)); }
        }

        private string _comment = string.Empty;
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnProp(nameof(Comment)); }
        }

        public MovementDialogViewModel(List<Product> products, Product? preselected)
        {
            Products = products;
            if (preselected != null) ProductId = preselected.Id;
        }

        // ── IDataErrorInfo ─────────────────────────────────────────────
        public string this[string col] => col switch
        {
            nameof(ProductId) => ProductId <= 0 ? "Выберите товар."                      : string.Empty,
            nameof(Quantity)  => Quantity  <= 0 ? "Количество должно быть больше нуля."  : string.Empty,
            _                 => string.Empty
        };
        public string Error   => this[nameof(ProductId)] + this[nameof(Quantity)];
        public bool   IsValid => string.IsNullOrEmpty(Error);

        public StockMovement ToMovement() => new()
        {
            ProductId = ProductId,
            Type      = SelectedType,
            Quantity  = Quantity,
            Comment   = Comment
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnProp(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
