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
            if (VM.SelectedProduct == null)
            {
                MessageBox.Show("Выберите товар.", "Валидация",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (VM.Quantity <= 0)
            {
                MessageBox.Show("Количество должно быть больше нуля.", "Валидация",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;
    }

    public class MovementDialogViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public List<Product>      Products      { get; }
        public List<MovementType> MovementTypes { get; } =
            new() { MovementType.In, MovementType.Out };

        // ── SelectedProduct (объект, не Id) ─────────────────────────
        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnProp(nameof(SelectedProduct)); }
        }

        private MovementType _selectedType = MovementType.In;
        public MovementType SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnProp(nameof(SelectedType)); }
        }

        private int _quantity = 1;
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
            Products        = products;
            _selectedProduct = preselected ?? (products.Count > 0 ? products[0] : null);
        }

        // IDataErrorInfo
        public string this[string col] => col switch
        {
            nameof(SelectedProduct) => SelectedProduct == null ? "Выберите товар." : string.Empty,
            nameof(Quantity)        => Quantity <= 0 ? "Должно быть > 0." : string.Empty,
            _ => string.Empty
        };
        public string Error   => this[nameof(SelectedProduct)] + this[nameof(Quantity)];
        public bool   IsValid => string.IsNullOrEmpty(Error);

        public StockMovement ToMovement() => new()
        {
            ProductId = SelectedProduct!.Id,
            Type      = SelectedType,
            Quantity  = Quantity,
            Comment   = Comment
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnProp(string n) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
