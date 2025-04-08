using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(BulletinBoardContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithSubcategoriesAsync()
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();
        }
    }
}