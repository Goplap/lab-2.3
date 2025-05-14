using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Mappers;
using BulletinBoard.BLL.Models;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL.Models;
using BulletinBoard.DAL.Repositories;
using BulletinBoard.DAL;
using lab_2._1.DAL.Interfaces;
using lab_2._1.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BulletinBoardContext>(options =>
            options.UseSqlServer(connectionString));

        // DAL
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAdRepository, AdRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Mappers
        services.AddScoped<IMapper<Ad, AdDto>, AdMapper>();
        services.AddScoped<IMapper<User, UserDto>, UserMapper>();
        services.AddScoped<IMapper<Category, CategoryDto>, CategoryMapper>();
        services.AddScoped<IMapper<Tag, TagDto>, TagMapper>();

        // BLL
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAdService, AdService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();

        return services;
    }
}
