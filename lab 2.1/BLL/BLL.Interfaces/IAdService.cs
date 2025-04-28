using BulletinBoard.BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Interfaces
{
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Interfaces
{
    public interface IAdService
    {
        Task<IEnumerable<AdDto>> GetAllAdsAsync();
        Task<AdDto> GetAdByIdAsync(int id);
        Task<IEnumerable<AdDto>> GetAdsByUserIdAsync(int userId);
        Task<IEnumerable<AdDto>> GetAdsByCategoryIdAsync(int categoryId);
        Task<AdDto> CreateAdAsync(AdDto ad);
        Task UpdateAdAsync(AdDto ad);
        Task DeleteAdAsync(int id);
    }
}
    public interface IAdService
    {
        Task<IEnumerable<AdDto>> GetAllAdsAsync();
        Task<AdDto> GetAdByIdAsync(int id);
        Task<IEnumerable<AdDto>> GetAdsByUserIdAsync(int userId);
        Task<IEnumerable<AdDto>> GetAdsByCategoryIdAsync(int categoryId);
        Task<AdDto> CreateAdAsync(AdDto ad);
        Task UpdateAdAsync(AdDto ad);
        Task DeleteAdAsync(int id);
    }
}