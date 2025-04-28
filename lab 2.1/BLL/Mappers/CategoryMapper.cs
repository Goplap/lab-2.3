using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace BulletinBoard.BLL.Mappers
{
    public class CategoryMapper : IMapper<Category, CategoryDto>
    {
        public CategoryDto Map(Category entity)
        {
            if (entity == null) return null;

            return new CategoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ParentCategoryId = entity.ParentCategoryId
            };
        }

        public Category Map(CategoryDto model)
        {
            if (model == null) return null;

            return new Category
            {
                Id = model.Id,
                Name = model.Name,
                ParentCategoryId = model.ParentCategoryId
            };
        }

        public IEnumerable<CategoryDto> Map(IEnumerable<Category> entities)
        {
            if (entities == null) return null;
            return entities.Select(Map);
        }

        public IEnumerable<Category> Map(IEnumerable<CategoryDto> models)
        {
            if (models == null) return null;
            return models.Select(Map);
        }
    }
}