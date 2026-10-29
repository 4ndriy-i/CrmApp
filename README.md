# CrmApp: CRM для обліку клієнтів і угод малого бізнесу

Лабораторна робота №1 (варіант 5). .NET MAUI, патерн MVVM, CommunityToolkit.Mvvm, Dependency Injection.

## Структура проєкту
- `Models/`: клас `Client` (Id, Name, Company, Phone, DealAmount)
- `ViewModels/`: `ClientsViewModel` (ObservableObject, RelayCommand)
- `Views/`: `ClientsPage` (XAML)
- `Services/`: `IClientService`, `InMemoryClientService`
- `Helpers/`: `InputParser`

## Можливості
- додавання клієнта з сумою угоди;
- видалення клієнта;
- автоматичний підрахунок загальної суми угод.

## Запуск
Відкрити рішення у Visual Studio 2022, вибрати Windows Machine або Android Emulator і натиснути Run.