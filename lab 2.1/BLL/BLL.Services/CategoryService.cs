using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(MapToCategoryDto);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category != null ? MapToCategoryDto(category) : null;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesWithSubcategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetCategoriesWithSubcategoriesAsync();
            return categories.Select(MapToCategoryDto);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto), "Категорія не може бути null.");

            var category = MapToCategory(categoryDto);

            // Перевірка на існування батьківської категорії, якщо вказана
            if (category.ParentCategoryId.HasValue)
            {
                var parentCategory = await _unitOfWork.Categories.GetByIdAsync(category.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new InvalidOperationException($"Батьківська категорія з ID {category.ParentCategoryId} не існує!");
                }
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToCategoryDto(category);
        }

        public async Task UpdateCategoryAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto), "Категорія не може бути null.");

            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null)
                throw new InvalidOperationException($"Категорія з ID {categoryDto.Id} не знайдена.");

            // Оновлюємо властивості існуючої категорії
            existingCategory.Name = categoryDto.Name;
            // Додайте інші властивості, які потрібно оновити

            // Якщо ParentCategoryId доступний у DTO, оновлюємо його також
            if (categoryDto.ParentCategoryId != null)
            {
                existingCategory.ParentCategoryId = categoryDto.ParentCategoryId;
            }

            await _unitOfWork.Categories.UpdateAsync(existingCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _unitOfWork.Categories.RemoveByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // Допоміжні методи для маппінгу
        private CategoryDto MapToCategoryDto(Category category)
        {
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
            };
        }

        private Category MapToCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null) return null;

            return new Category
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name,
                ParentCategoryId = categoryDto.ParentCategoryId,
            };
        }
    }
}