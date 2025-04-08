using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(BulletinBoardContext context) : base(context)
        {
        }

        public async Task<Tag> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}