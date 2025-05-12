using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Services
{
    public class AdService : IAdService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<Ad, AdDto> _mapper;

        public AdService(IUnitOfWork unitOfWork, IMapper<Ad, AdDto> mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdDto>> GetAllAdsAsync()
        {
            var ads = await _unitOfWork.Ads.GetAllAsync();
            return _mapper.Map(ads);
        }

        public async Task<AdDto> GetAdByIdAsync(int id)
        {
            var ad = await _unitOfWork.Ads.GetByIdAsync(id);
            return _mapper.Map(ad);
        }

        public async Task<IEnumerable<AdDto>> GetAdsByUserIdAsync(int userId)
        {
            var ads = await _unitOfWork.Ads.GetAdsByUserIdAsync(userId);
            return _mapper.Map(ads);
        }

        public async Task<IEnumerable<AdDto>> GetAdsByCategoryIdAsync(int categoryId)
        {
            var ads = await _unitOfWork.Ads.GetAdsByCategoryIdAsync(categoryId);
            return _mapper.Map(ads);
        }

        public async Task<AdDto> CreateAdAsync(AdDto adDto)
        {
            if (adDto == null)
                throw new ArgumentNullException(nameof(adDto), "Оголошення не може бути null.");

            // Validations 
            var category = await _unitOfWork.Categories.GetByIdAsync(adDto.CategoryId);
            if (category == null)
                throw new InvalidOperationException($"Категорія з ID {adDto.CategoryId} не існує!");

            var user = await _unitOfWork.Users.GetByIdAsync(adDto.UserId);
            if (user == null)
                throw new InvalidOperationException($"Користувач з ID {adDto.UserId} не існує!");

            // Default values
            adDto.CreatedAt = DateTime.UtcNow;
            adDto.IsActive = true;

            var ad = _mapper.Map(adDto);
            await _unitOfWork.Ads.AddAsync(ad);
            await _unitOfWork.SaveChangesAsync();

            adDto.Id = ad.Id;
            return adDto;
        }

        public async Task UpdateAdAsync(AdDto adDto)
        {
            if (adDto == null)
                throw new ArgumentNullException(nameof(adDto), "Оголошення не може бути null.");

            var ad = _mapper.Map(adDto);
            await _unitOfWork.Ads.UpdateAsync(ad);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAdAsync(int id)
        {
            await _unitOfWork.Ads.RemoveByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}