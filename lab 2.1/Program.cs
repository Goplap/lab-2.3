using System;
using System.Threading.Tasks;
using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL;
using BulletinBoard.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        var connectionString = "Server=localhost\\SQLEXPRESS;Database=BulletinBoardDB;Trusted_Connection=True;TrustServerCertificate=True;";
        Console.WriteLine($"Використовую рядок підключення: {connectionString}");

        var host = CreateHost(connectionString);

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<BulletinBoardContext>();
                Console.WriteLine("Застосування міграцій...");
                context.Database.Migrate();
                Console.WriteLine("База даних оновлена та готова до роботи.");
            }
            catch (Exception ex)
            {
                LogError("Помилка під час застосування міграцій!", ex);
                return;
            }

            var userService = services.GetRequiredService<IUserService>();
            var adService = services.GetRequiredService<IAdService>();
            var categoryService = services.GetRequiredService<ICategoryService>();

            await RunUI(userService, adService, categoryService);
        }
    }

    static IHost CreateHost(string connectionString) =>
        Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddDbContext<BulletinBoardContext>(options =>
                    options.UseSqlServer(connectionString));
                services.AddScoped<IAdService, AdService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<ICategoryService, CategoryService>();
            })
            .Build();

    static async Task RunUI(IUserService userService, IAdService adService, ICategoryService categoryService)
    {
        while (true)
        {
            Console.WriteLine("\n1. Додати користувача");
            Console.WriteLine("2. Додати оголошення");
            Console.WriteLine("3. Показати всіх користувачів");
            Console.WriteLine("4. Показати всі оголошення");
            Console.WriteLine("5. Редагувати оголошення");
            Console.WriteLine("6. Видалити оголошення");
            Console.WriteLine("7. Додати категорію");
            Console.WriteLine("8. Вийти");
            Console.Write("Ваш вибір: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Ім'я користувача: ");
                    string name = Console.ReadLine();
                    Console.Write("Email: ");
                    string email = Console.ReadLine();
                    await userService.CreateUserAsync(new User { Username = name, Email = email });
                    Console.WriteLine("✅ Користувач доданий!");
                    break;

                case "2":
                    Console.Write("Тема оголошення: ");
                    string title = Console.ReadLine();
                    Console.Write("Текст оголошення: ");
                    string text = Console.ReadLine();
                    
                    Console.Write("Виберіть категорію (ID): ");
                    if (!int.TryParse(Console.ReadLine(), out int categoryId))
                    {
                        Console.WriteLine("❌ Некоректний ID категорії!");
                        break;
                    }

                    // Перевіряємо, чи існує категорія
                    var category = await categoryService.GetCategoryByIdAsync(categoryId);
                    if (category == null)
                    {
                        Console.WriteLine($"❌ Категорія з ID {categoryId} не існує! Додайте категорію перед створенням оголошення.");
                        break;
                    }

                    try
                    {
                        await adService.CreateAdAsync(new Ad { Title = title, Description = text, UserId = 1, CategoryId = categoryId });
                        Console.WriteLine("✅ Оголошення додане!");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка під час додавання оголошення!", ex);
                    }
                    break;

                case "3":
                    var users = await userService.GetAllUsersAsync();
                    foreach (var user in users)
                    {
                        Console.WriteLine($"ID: {user.Id}, Ім'я: {user.Username}, Email: {user.Email}");
                    }
                    break;

                case "4":
                    var ads = await adService.GetAllAdsAsync();
                    foreach (var ad in ads)
                    {
                        Console.WriteLine($"ID: {ad.Id}, Заголовок: {ad.Title}, Опис: {ad.Description}, Категорія: {ad.Category?.Name}");
                    }
                    break;

                case "5":
                    Console.Write("Введіть ID оголошення для редагування: ");
                    if (!int.TryParse(Console.ReadLine(), out int adIdToEdit))
                    {
                        Console.WriteLine("❌ Некоректний ID!");
                        break;
                    }

                    var adToEdit = await adService.GetAdByIdAsync(adIdToEdit);
                    if (adToEdit != null)
                    {
                        Console.Write("Нова тема оголошення: ");
                        adToEdit.Title = Console.ReadLine();
                        Console.Write("Новий текст оголошення: ");
                        adToEdit.Description = Console.ReadLine();
                        await adService.UpdateAdAsync(adToEdit);
                        Console.WriteLine("✅ Оголошення оновлено!");
                    }
                    else
                    {
                        Console.WriteLine("❌ Оголошення не знайдено!");
                    }
                    break;

                case "6":
                    Console.Write("Введіть ID оголошення для видалення: ");
                    if (!int.TryParse(Console.ReadLine(), out int adIdToDelete))
                    {
                        Console.WriteLine("❌ Некоректний ID!");
                        break;
                    }

                    await adService.DeleteAdAsync(adIdToDelete);
                    Console.WriteLine("✅ Оголошення видалене!");
                    break;

                case "7":
                    Console.Write("Введіть назву категорії: ");
                    string categoryName = Console.ReadLine();
                    await categoryService.CreateCategoryAsync(new Category { Name = categoryName });
                    Console.WriteLine("✅ Категорія додана!");
                    break;

                case "8":
                    return;

                default:
                    Console.WriteLine("❌ Невірний вибір!");
                    break;
            }
        }
    }

    static void LogError(string message, Exception ex)
    {
        Console.WriteLine($"❌ {message}");
        Console.WriteLine($"Помилка: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Деталі: {ex.InnerException.Message}");
        }
    }
}
