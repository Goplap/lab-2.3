using System.Threading.Tasks;
using BulletinBoard.DAL;
using BulletinBoard.DAL.Models;
using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
    private readonly BulletinBoardContext _context;

    public CategoryService(BulletinBoardContext context)
    {
        _context = context;
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task CreateCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }
}
