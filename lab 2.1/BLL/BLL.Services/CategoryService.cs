using BulletinBoard.BLL.Interfaces;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithSubcategoriesAsync()
        {
            return await _unitOfWork.Categories.GetCategoriesWithSubcategoriesAsync();
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category), "Категорія не може бути null.");

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

            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category), "Категорія не може бути null.");

            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _unitOfWork.Categories.RemoveByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
