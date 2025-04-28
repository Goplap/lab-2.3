using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.DAL.Models;

namespace BulletinBoard.BLL.Mappers
{
    public class AdMapper : IMapper<Ad, AdDto>
    {
        private readonly IMapper<User, UserDto> _userMapper;
        private readonly IMapper<Category, CategoryDto> _categoryMapper;
        private readonly IMapper<Tag, TagDto> _tagMapper;

        public AdMapper(
        IMapper<User, UserDto> userMapper,
            IMapper<Category, CategoryDto> categoryMapper,
            IMapper<Tag, TagDto> tagMapper)
        {
            _userMapper = userMapper;
            _categoryMapper = categoryMapper;
            _tagMapper = tagMapper;
        }

        public AdDto Map(Ad entity)
        {
            if (entity == null) return null;

            return new AdDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsActive = entity.IsActive,
                UserId = entity.UserId,
                CategoryId = entity.CategoryId,
                User = entity.User != null ? _userMapper.Map(entity.User) : null,
                Category = entity.Category != null ? _categoryMapper.Map(entity.Category) : null,
                Tags = entity.Tags != null ? _tagMapper.Map(entity.Tags).ToList() : new List<TagDto>()
            };
        }

        public Ad Map(AdDto model)
        {
            if (model == null) return null;

            return new Ad
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                IsActive = model.IsActive,
                UserId = model.UserId,
                CategoryId = model.CategoryId
                // В Ad не встановлюємо навігаційні властивості, бо їх встановлює EF
            };
        }

        public IEnumerable<AdDto> Map(IEnumerable<Ad> entities)
        {
            if (entities == null) return null;
            return entities.Select(Map);
        }

        public IEnumerable<Ad> Map(IEnumerable<AdDto> models)
        {
            if (models == null) return null;
            return models.Select(Map);
        }
    }
}