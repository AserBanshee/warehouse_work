using System.ComponentModel;

namespace InventoryApp.Validators
{
    /// <summary>
    /// Валидатор для формы добавления/редактирования товара.
    /// Реализует IDataErrorInfo — стандартный механизм WPF-валидации.
    /// </summary>
    public class ProductValidator : IDataErrorInfo
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int CategoryId { get; set; }

        public string this[string columnName] => columnName switch
        {
            nameof(Name)       => string.IsNullOrWhiteSpace(Name) ? "Название не может быть пустым." : string.Empty,
            nameof(SKU)        => string.IsNullOrWhiteSpace(SKU)  ? "SKU обязателен."               : string.Empty,
            nameof(Quantity)   => Quantity < 0                    ? "Остаток не может быть отрицательным." : string.Empty,
            nameof(CategoryId) => CategoryId <= 0                 ? "Выберите категорию."            : string.Empty,
            _                  => string.Empty
        };

        public string Error =>
            this[nameof(Name)] + this[nameof(SKU)] +
            this[nameof(Quantity)] + this[nameof(CategoryId)];

        public bool IsValid => string.IsNullOrEmpty(Error);
    }

    /// <summary>
    /// Валидатор для формы регистрации движения товара.
    /// </summary>
    public class MovementValidator : IDataErrorInfo
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public string this[string columnName] => columnName switch
        {
            nameof(ProductId) => ProductId <= 0  ? "Выберите товар."                    : string.Empty,
            nameof(Quantity)  => Quantity <= 0   ? "Количество должно быть больше нуля." : string.Empty,
            _                 => string.Empty
        };

        public string Error => this[nameof(ProductId)] + this[nameof(Quantity)];
        public bool IsValid => string.IsNullOrEmpty(Error);
    }
}
