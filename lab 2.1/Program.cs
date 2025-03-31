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

            await RunUI(userService, adService);
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
            })
            .Build();

    static async Task RunUI(IUserService userService, IAdService adService)
    {
        while (true)
        {
            Console.WriteLine("\n1. Додати користувача");
            Console.WriteLine("2. Додати оголошення");
            Console.WriteLine("3. Показати всіх користувачів");
            Console.WriteLine("4. Вийти");
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
                    Console.WriteLine("Користувач доданий!");
                    break;

                case "2":
                    Console.Write("Тема оголошення: ");
                    string title = Console.ReadLine();
                    Console.Write("Текст оголошення: ");
                    string text = Console.ReadLine();
                    await adService.CreateAdAsync(new Ad { Title = title, Description = text, UserId = 1 }); // ID 1 для тесту
                    Console.WriteLine("Оголошення додане!");
                    break;

                case "3":
                    var users = await userService.GetAllUsersAsync();
                    foreach (var user in users)
                    {
                        Console.WriteLine($"ID: {user.Id}, Ім'я: {user.Username}, Email: {user.Email}");
                    }
                    break;

                case "4":
                    return;

                default:
                    Console.WriteLine("Невірний вибір!");
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
