using BulletinBoard.DAL.Models;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag> GetByNameAsync(string name);
    }
}