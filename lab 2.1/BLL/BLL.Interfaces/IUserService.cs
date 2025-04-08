using BulletinBoard.DAL.Models;

namespace BulletinBoard.BLL.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);  // Corrected return type
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}