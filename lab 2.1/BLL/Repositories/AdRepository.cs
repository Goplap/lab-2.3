using BulletinBoard.DAL.Interfaces;
using BulletinBoard.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Repositories
{
    public class AdRepository : Repository<Ad>, IAdRepository
    {
        public AdRepository(BulletinBoardContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ad>> GetAdsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ad>> GetAdsByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Where(a => a.CategoryId == categoryId)
                .Include(a => a.User)
                .Include(a => a.Tags)
                .ToListAsync();
        }

        public async Task<Ad> GetAdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Ad>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .ToListAsync();
        }
    }
}