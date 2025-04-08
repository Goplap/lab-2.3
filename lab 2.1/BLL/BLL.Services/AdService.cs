using BulletinBoard.BLL.Interfaces;
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

        public AdService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Ad>> GetAllAdsAsync()
        {
            return await _unitOfWork.Ads.GetAllWithDetailsAsync();
        }

        public async Task<Ad> GetAdByIdAsync(int id)
        {
            return await _unitOfWork.Ads.GetAdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Ad>> GetAdsByUserIdAsync(int userId)
        {
            return await _unitOfWork.Ads.GetAdsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Ad>> GetAdsByCategoryIdAsync(int categoryId)
        {
            return await _unitOfWork.Ads.GetAdsByCategoryIdAsync(categoryId);
        }

        public async Task<Ad> CreateAdAsync(Ad ad)
        {
            if (ad == null)
                throw new ArgumentNullException(nameof(ad), "Оголошення не може бути null.");

            // Перевірка існування категорії
            var category = await _unitOfWork.Categories.GetByIdAsync(ad.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Категорія з ID {ad.CategoryId} не існує! Додайте категорію перед створенням оголошення.");
            }

            // Перевірка існування користувача
            var user = await _unitOfWork.Users.GetByIdAsync(ad.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"Користувач з ID {ad.UserId} не існує! Створіть користувача перед додаванням оголошення.");
            }

            // Встановлення дефолтних значень
            ad.CreatedAt = DateTime.UtcNow;
            ad.IsActive = true;

            await _unitOfWork.Ads.AddAsync(ad);
            await _unitOfWork.SaveChangesAsync();

            return ad;
        }

        public async Task UpdateAdAsync(Ad ad)
        {
            if (ad == null)
                throw new ArgumentNullException(nameof(ad), "Оголошення не може бути null.");

            // Перевірка існування категорії
            var category = await _unitOfWork.Categories.GetByIdAsync(ad.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Категорія з ID {ad.CategoryId} не існує! Оновлення неможливе.");
            }

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
