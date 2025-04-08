using BulletinBoard.DAL.Models;
using System.Threading.Tasks;

namespace lab_2._1.DAL.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag> GetByNameAsync(string name);
    }
}