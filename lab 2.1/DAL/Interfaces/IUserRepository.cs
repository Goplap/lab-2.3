using BulletinBoard.DAL.Models;
using System.Threading.Tasks;

namespace lab_2._1.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }
}