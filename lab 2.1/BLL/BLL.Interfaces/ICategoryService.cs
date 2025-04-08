using System.Threading.Tasks;
using BulletinBoard.DAL.Models;

public interface ICategoryService
{
    Task<Category> GetCategoryByIdAsync(int id);
    Task CreateCategoryAsync(Category category);
}
