using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace BulletinBoard.BLL.Mappers
{
    public class UserMapper : IMapper<User, UserDto>
    {
        public UserDto Map(User entity)
        {
            if (entity == null) return null;

            return new UserDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                RegisteredAt = entity.RegisteredAt,
                IsActive = entity.IsActive
            };
        }

        public User Map(UserDto model)
        {
            if (model == null) return null;

            return new User
            {
                Id = model.Id,
                Username = model.Username,
                Email = model.Email,
                RegisteredAt = model.RegisteredAt,
                IsActive = model.IsActive
            };
        }

        public IEnumerable<UserDto> Map(IEnumerable<User> entities)
        {
            if (entities == null) return null;
            return entities.Select(Map);
        }

        public IEnumerable<User> Map(IEnumerable<UserDto> models)
        {
            if (models == null) return null;
            return models.Select(Map);
        }
    }
}