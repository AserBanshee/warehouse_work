# InventoryApp — Учёт ТМЦ (WPF + EF Core + SQLite)

## Структура проекта

```
InventoryApp/
├── Models/
│   ├── Product.cs          — товар (Id, Name, SKU, Quantity, CategoryId)
│   ├── Category.cs         — категория
│   ├── StockMovement.cs    — движение товара (In / Out)
│   └── User.cs             — пользователь (опционально)
├── Data/
│   └── AppDbContext.cs     — EF Core контекст, SQLite, seed данных
├── Services/
│   ├── IServices.cs        — интерфейсы (SOLID: DIP)
│   └── Services.cs         — реализации ProductService, CategoryService,
│                             StockMovementService (атомарное обновление остатков)
├── Commands/
│   └── RelayCommand.cs     — универсальный ICommand
├── ViewModels/
│   ├── BaseViewModel.cs    — INotifyPropertyChanged
│   └── MainViewModel.cs    — главная ViewModel (ObservableCollections, команды)
├── Views/
│   ├── MainWindow.xaml     — два Grid: левый=категории, правый=контент
│   ├── MainWindow.xaml.cs
│   ├── ProductDialog.xaml  — диалог добавления/редактирования товара
│   ├── ProductDialog.xaml.cs — ProductDialogViewModel с IDataErrorInfo
│   ├── MovementDialog.xaml — диалог прихода/расхода
│   └── MovementDialog.xaml.cs — MovementDialogViewModel с IDataErrorInfo
├── Validators/
│   └── Validators.cs       — ProductValidator, MovementValidator
├── Migrations/
│   └── InitialMigration.cs — шаблон (генерировать через dotnet ef)
├── App.xaml                — стили (Button, TextBox, ListView)
├── App.xaml.cs             — Bootstrap: DB migrate + DI wiring
└── InventoryApp.csproj
```

## Быстрый старт

```bash
# 1. Восстановить пакеты
dotnet restore

# 2. Создать миграцию (если ещё не создана)
dotnet ef migrations add Initial

# 3. Применить миграцию (создаст inventory.db автоматически при запуске)
#    — OR — просто запустите приложение, db.Database.Migrate() вызывается в App.xaml.cs

# 4. Запустить
dotnet run
```

## Архитектурные решения (SOLID)

| Принцип | Где применён |
|---------|-------------|
| **S** — Single Responsibility | Каждый сервис отвечает за одну сущность; ViewModel — только за UI-логику |
| **O** — Open/Closed | Новые команды добавляются без изменения существующих |
| **L** — Liskov | Все сервисы взаимозаменяемы через интерфейсы |
| **I** — Interface Segregation | Три отдельных интерфейса: IProductService, ICategoryService, IStockMovementService |
| **D** — Dependency Inversion | MainViewModel зависит от интерфейсов, не от конкретных классов |

## Паттерны WPF

- **MVVM**: View ↔ ViewModel через Binding, код-бихайнд минимален  
- **ObservableCollection<T>**: реализованы как полные свойства с OnPropertyChanged  
- **ICommand**: RelayCommand с параметрами execute/canExecute  
- **IDataErrorInfo**: валидация в ProductDialogViewModel и MovementDialogViewModel  
- **Binding с ValidatesOnDataErrors=True**: визуальная подсветка ошибок

## Бизнес-логика остатков

```
StockMovementService.RegisterMovementAsync:
  1. Найти товар по ProductId
  2. Если расход — проверить достаточность остатка
  3. Атомарно обновить Product.Quantity
  4. Записать StockMovement с текущей датой
  5. SaveChangesAsync (транзакция EF Core)
```
