using BulletinBoard.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Interfaces
{
    public interface IAdService
    {
        Task<IEnumerable<Ad>> GetAllAdsAsync();
        Task<Ad> GetAdByIdAsync(int id);
        Task<IEnumerable<Ad>> GetAdsByUserIdAsync(int userId);
        Task<IEnumerable<Ad>> GetAdsByCategoryIdAsync(int categoryId);
        Task<Ad> CreateAdAsync(Ad ad);
        Task UpdateAdAsync(Ad ad);
        Task DeleteAdAsync(int id);
    }
}