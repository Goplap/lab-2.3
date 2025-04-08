using BulletinBoard.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab_2._1.DAL.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithSubcategoriesAsync();
    }
}