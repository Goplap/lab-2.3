using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Mappers;
using BulletinBoard.BLL.Models;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL;
using BulletinBoard.DAL.Models;
using BulletinBoard.DAL.Repositories;
using lab_2._1.DAL.Interfaces;
using lab_2._1.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure database connection
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=localhost\\SQLEXPRESS;Database=BulletinBoardDB;Trusted_Connection=True;TrustServerCertificate=True;";

// Register the specific BulletinBoardContext instead of generic DbContext
builder.Services.AddDbContext<BulletinBoardContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdRepository, AdRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Register mappers
builder.Services.AddScoped<IMapper<Ad, AdDto>, AdMapper>();
builder.Services.AddScoped<IMapper<User, UserDto>, UserMapper>();
builder.Services.AddScoped<IMapper<Category, CategoryDto>, CategoryMapper>();
builder.Services.AddScoped<IMapper<Tag, TagDto>, TagMapper>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdService, AdService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bulletin Board API",
        Version = "v1",
        Description = "API for Bulletin Board Application"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bulletin Board API V1");
        // Use "swagger" as the route prefix instead of empty string
        c.RoutePrefix = "swagger";
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BulletinBoardContext>();
    context.Database.Migrate();
    Console.WriteLine("Database has been migrated and is ready for use.");
}

app.Run();