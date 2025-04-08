using BulletinBoard.DAL.Interfaces;
using BulletinBoard.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BulletinBoardContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}