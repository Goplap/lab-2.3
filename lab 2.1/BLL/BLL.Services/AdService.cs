using BulletinBoard.BLL.Interfaces;
using BulletinBoard.DAL.Models;

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
            // Тут студент може використовувати простий підхід без оптимізації
            var repository = _unitOfWork.GetRepository<Ad>();
            var ads = await repository.GetAllAsync();

            // В реальному проекті тут мала б бути логіка з Include(..), але це "студентська робота"
            return _mapper.Map(ads);
        }

        public async Task<AdDto> GetAdByIdAsync(int id)
        {
            var repository = _unitOfWork.GetRepository<Ad>();
            var ad = await repository.GetByIdAsync(id);

            // Спрощений варіант без зв'язків
            return _mapper.Map(ad);
        }

        public async Task<IEnumerable<AdDto>> GetAdsByUserIdAsync(int userId)
        {
            var repository = _unitOfWork.GetRepository<Ad>();
            var ads = await repository.FindAsync(a => a.UserId == userId);
            return _mapper.Map(ads);
        }

        public async Task<IEnumerable<AdDto>> GetAdsByCategoryIdAsync(int categoryId)
        {
            var repository = _unitOfWork.GetRepository<Ad>();
            var ads = await repository.FindAsync(a => a.CategoryId == categoryId);
            return _mapper.Map(ads);
        }

        public async Task<AdDto> CreateAdAsync(AdDto adDto)
        {
            if (adDto == null)
                throw new ArgumentNullException(nameof(adDto), "Оголошення не може бути null.");

            // Простий метод перевірки без складної логіки
            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(adDto.CategoryId);

            if (category == null)
                throw new InvalidOperationException($"Категорія з ID {adDto.CategoryId} не існує!");

            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(adDto.UserId);

            if (user == null)
                throw new InvalidOperationException($"Користувач з ID {adDto.UserId} не існує!");

            // Встановлення дефолтних значень
            adDto.CreatedAt = DateTime.UtcNow;
            adDto.IsActive = true;

            var ad = _mapper.Map(adDto);
            var adRepo = _unitOfWork.GetRepository<Ad>();
            await adRepo.AddAsync(ad);
            await _unitOfWork.SaveChangesAsync();

            adDto.Id = ad.Id; // Оновлення ідентифікатора після збереження в БД
            return adDto;
        }

        public async Task UpdateAdAsync(AdDto adDto)
        {
            if (adDto == null)
                throw new ArgumentNullException(nameof(adDto), "Оголошення не може бути null.");

            var adRepo = _unitOfWork.GetRepository<Ad>();
            var ad = _mapper.Map(adDto);

            await adRepo.UpdateAsync(ad);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAdAsync(int id)
        {
            var adRepo = _unitOfWork.GetRepository<Ad>();
            await adRepo.DeleteByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}