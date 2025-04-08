using BulletinBoard.DAL.Models;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }
}