using BulletinBoard.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesWithSubcategoriesAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
    }
}