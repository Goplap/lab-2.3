using System;
using System.Threading.Tasks;
using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Mappers;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL;
using BulletinBoard.DAL.Models;
using BulletinBoard.DAL.Repositories;
using lab_2._1.DAL.Interfaces;
using lab_2._1.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        var host = CreateHost();

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
            var tagService = services.GetRequiredService<ITagService>();

            await RunUI(userService, adService, categoryService, tagService);
        }
    }

    static IHost CreateHost(string connectionString) =>
    Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            // Реєстрація DbContext
            services.AddDbContext<BulletinBoardContext>(options =>
                options.UseSqlServer(connectionString));

            // Реєстрація UnitOfWork
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // Реєстрація мапперів
            services.AddScoped<IMapper<User, UserDto>, UserMapper>();
            services.AddScoped<IMapper<Category, CategoryDto>, CategoryMapper>();
            services.AddScoped<IMapper<Tag, TagDto>, TagMapper>();
            services.AddScoped<IMapper<Ad, AdDto>, AdMapper>();

            // Реєстрація сервісів
            services.AddScoped<IAdService, AdService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
        })
        .Build();

    static async Task RunUI(IUserService userService, IAdService adService, ICategoryService categoryService, ITagService tagService)
    {
        User currentUser = null;

        // Спочатку перевіримо, чи є хоча б один користувач, і створимо його, якщо потрібно
        try
        {
            var users = await userService.GetAllUsersAsync();

            if (!users.GetEnumerator().MoveNext()) // якщо немає користувачів
            {
                Console.WriteLine("Створення першого користувача...");
                currentUser = await userService.CreateUserAsync(new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "admin"
                });
                Console.WriteLine($"Створено користувача: {currentUser.Username}");
            }
            else
            {
                currentUser = users.GetEnumerator().Current; // беремо першого користувача
            }
        }
        catch (Exception ex)
        {
            LogError("Помилка при перевірці/створенні користувачів", ex);
            return;
        }

        while (true)
        {
            Console.WriteLine("\n===== МЕНЮ ДОШКИ ОГОЛОШЕНЬ =====");
            Console.WriteLine($"Поточний користувач: {currentUser?.Username ?? "Не авторизовано"}");
            Console.WriteLine("1. Додати користувача");
            Console.WriteLine("2. Додати оголошення");
            Console.WriteLine("3. Показати всіх користувачів");
            Console.WriteLine("4. Показати всі оголошення");
            Console.WriteLine("5. Редагувати оголошення");
            Console.WriteLine("6. Видалити оголошення");
            Console.WriteLine("7. Додати категорію");
            Console.WriteLine("8. Показати категорії");
            Console.WriteLine("9. Змінити поточного користувача");
            Console.WriteLine("0. Вийти");
            Console.Write("Ваш вибір: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": // Додати користувача
                    try
                    {
                        Console.Write("Ім'я користувача: ");
                        string name = Console.ReadLine();
                        Console.Write("Email: ");
                        string email = Console.ReadLine();
                        Console.Write("Пароль: ");
                        string password = Console.ReadLine();

                        var newUser = await userService.CreateUserAsync(new User
                        {
                            Username = name,
                            Email = email,
                            PasswordHash = password
                        });

                        Console.WriteLine($"✅ Користувач '{name}' доданий з ID: {newUser.Id}!");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при створенні користувача!", ex);
                    }
                    break;

                case "2": // Додати оголошення
                    if (currentUser == null)
                    {
                        Console.WriteLine("❌ Спочатку виберіть користувача!");
                        break;
                    }

                    try
                    {
                        // Показуємо доступні категорії
                        var categories = await categoryService.GetAllCategoriesAsync();
                        Console.WriteLine("\nДоступні категорії:"); foreach (var cat in categories)
                        {
                            Console.WriteLine($"{cat.Id}. {cat.Name}");
                        }

                        Console.Write("\nТема оголошення: ");
                        string title = Console.ReadLine();
                        Console.Write("Текст оголошення: ");
                        string text = Console.ReadLine();

                        Console.Write("Виберіть категорію (ID): ");
                        if (!int.TryParse(Console.ReadLine(), out int categoryId))
                        {
                            Console.WriteLine("❌ Некоректний ID категорії!");
                            break;
                        }

                        // Створюємо оголошення з прив'язкою до поточного користувача
                        var ad = await adService.CreateAdAsync(new Ad
                        {
                            Title = title,
                            Description = text,
                            UserId = currentUser.Id,
                            CategoryId = categoryId
                        });

                        Console.WriteLine($"✅ Оголошення '{title}' додане з ID: {ad.Id}!");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка під час додавання оголошення!", ex);
                    }
                    break;

                case "3": // Показати всіх користувачів
                    try
                    {
                        var users = await userService.GetAllUsersAsync();
                        Console.WriteLine("\n=== СПИСОК КОРИСТУВАЧІВ ===");
                        foreach (var user in users)
                        {
                            Console.WriteLine($"ID: {user.Id}, Ім'я: {user.Username}, Email: {user.Email}");
                        }
                        Console.WriteLine("=========================");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при отриманні списку користувачів", ex);
                    }
                    break;

                case "4": // Показати всі оголошення
                    try
                    {
                        var ads = await adService.GetAllAdsAsync();
                        Console.WriteLine("\n=== СПИСОК ОГОЛОШЕНЬ ===");
                        foreach (var ad in ads)
                        {
                            Console.WriteLine($"ID: {ad.Id}, Заголовок: {ad.Title}");
                            Console.WriteLine($"Автор: {ad.User?.Username ?? "Невідомий"}, Категорія: {ad.Category?.Name ?? "Без категорії"}");
                            Console.WriteLine($"Опис: {ad.Description}");
                            Console.WriteLine($"Дата створення: {ad.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                            Console.WriteLine("-------------------------");
                        }
                        Console.WriteLine("=======================");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при отриманні списку оголошень", ex);
                    }
                    break;

                case "5": // Редагувати оголошення
                    try
                    {
                        Console.Write("Введіть ID оголошення для редагування: ");
                        if (!int.TryParse(Console.ReadLine(), out int adIdToEdit))
                        {
                            Console.WriteLine("❌ Некоректний ID!");
                            break;
                        }

                        var adToEdit = await adService.GetAdByIdAsync(adIdToEdit);
                        if (adToEdit == null)
                        {
                            Console.WriteLine("❌ Оголошення не знайдено!");
                            break;
                        }

                        // Перевіряємо чи користувач є автором оголошення
                        if (adToEdit.UserId != currentUser.Id)
                        {
                            Console.WriteLine("❌ Ви можете редагувати тільки власні оголошення!");
                            break;
                        }

                        Console.Write($"Нова тема оголошення [{adToEdit.Title}]: ");
                        string newTitle = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newTitle))
                            adToEdit.Title = newTitle;

                        Console.Write($"Новий текст оголошення [{adToEdit.Description}]: ");
                        string newDescription = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newDescription))
                            adToEdit.Description = newDescription;

                        // Показуємо доступні категорії
                        var categories = await categoryService.GetAllCategoriesAsync();
                        Console.WriteLine("\nДоступні категорії:");
                        foreach (var cat in categories)
                        {
                            Console.WriteLine($"{cat.Id}. {cat.Name}");
                        }

                        Console.Write($"Нова категорія (ID) [{adToEdit.CategoryId}]: ");
                        string categoryInput = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(categoryInput) && int.TryParse(categoryInput, out int newCategoryId))
                        {
                            var category = await categoryService.GetCategoryByIdAsync(newCategoryId);
                            if (category != null)
                                adToEdit.CategoryId = newCategoryId;
                            else
                                Console.WriteLine($"❌ Категорія з ID {newCategoryId} не існує. Категорія залишена без змін.");
                        }

                        await adService.UpdateAdAsync(adToEdit);
                        Console.WriteLine("✅ Оголошення оновлено!");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при редагуванні оголошення", ex);
                    }
                    break;

                case "6": // Видалити оголошення
                    try
                    {
                        Console.Write("Введіть ID оголошення для видалення: ");
                        if (!int.TryParse(Console.ReadLine(), out int adIdToDelete))
                        {
                            Console.WriteLine("❌ Некоректний ID!");
                            break;
                        }

                        var adToDelete = await adService.GetAdByIdAsync(adIdToDelete);
                        if (adToDelete == null)
                        {
                            Console.WriteLine("❌ Оголошення не знайдено!");
                            break;
                        }

                        // Перевіряємо чи користувач є автором оголошення
                        if (adToDelete.UserId != currentUser.Id)
                        {
                            Console.WriteLine("❌ Ви можете видаляти тільки власні оголошення!");
                            break;
                        }

                        Console.Write($"Ви впевнені, що хочете видалити оголошення '{adToDelete.Title}'? (так/ні): ");
                        if (Console.ReadLine().ToLower() == "так")
                        {
                            await adService.DeleteAdAsync(adIdToDelete);
                            Console.WriteLine("✅ Оголошення видалене!");
                        }
                        else
                        {
                            Console.WriteLine("Видалення скасовано.");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при видаленні оголошення", ex);
                    }
                    break;

                case "7": // Додати категорію
                    try
                    {
                        // Показуємо існуючі категорії
                        var existingCategories = await categoryService.GetAllCategoriesAsync();
                        Console.WriteLine("\nІснуючі категорії:");
                        foreach (var cat in existingCategories)
                        {
                            Console.WriteLine($"{cat.Id}. {cat.Name}");
                        }

                        Console.Write("\nВведіть назву нової категорії: ");
                        string categoryName = Console.ReadLine();

                        Console.Write("Введіть ID батьківської категорії (або 0, якщо це головна категорія): ");
                        int.TryParse(Console.ReadLine(), out int parentId);

                        Category newCategory = new Category { Name = categoryName };

                        if (parentId > 0)
                        {
                            newCategory.ParentCategoryId = parentId;
                        }

                        var category = await categoryService.CreateCategoryAsync(newCategory);
                        Console.WriteLine($"✅ Категорія '{categoryName}' додана з ID: {category.Id}!");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при створенні категорії", ex);
                    }
                    break;

                case "8": // Показати категорії
                    try
                    {
                        var allCategories = await categoryService.GetAllCategoriesAsync();
                        Console.WriteLine("\n=== СПИСОК КАТЕГОРІЙ ===");
                        foreach (var category in allCategories)
                        {
                            string parentInfo = category.ParentCategoryId.HasValue
                                ? $" (Підкатегорія {category.ParentCategoryId})"
                                : "";
                            Console.WriteLine($"ID: {category.Id}, Назва: {category.Name}{parentInfo}");
                        }
                        Console.WriteLine("=======================");
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при отриманні списку категорій", ex);
                    }
                    break;

                case "9": // Змінити поточного користувача
                    try
                    {
                        var users = await userService.GetAllUsersAsync();
                        Console.WriteLine("\n=== ВИБЕРІТЬ КОРИСТУВАЧА ===");
                        foreach (var user in users)
                        {
                            Console.WriteLine($"{user.Id}. {user.Username} ({user.Email})");
                        }

                        Console.Write("Введіть ID користувача: ");
                        if (int.TryParse(Console.ReadLine(), out int userId))
                        {
                            var user = await userService.GetUserByIdAsync(userId);
                            if (user != null)
                            {
                                currentUser = user;
                                Console.WriteLine($"✅ Поточний користувач змінений на {user.Username}");
                            }
                            else
                            {
                                Console.WriteLine("❌ Користувача з таким ID не знайдено!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("❌ Некоректний ID!");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("Помилка при зміні користувача", ex);
                    }
                    break;

                case "0": // Вийти
                    Console.WriteLine("До побачення!");
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