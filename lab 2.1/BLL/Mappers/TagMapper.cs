using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace BulletinBoard.BLL.Mappers
{
    public class TagMapper : IMapper<Tag, TagDto>
    {
        public TagDto Map(Tag entity)
        {
            if (entity == null) return null;

            return new TagDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public Tag Map(TagDto model)
        {
            if (model == null) return null;

            return new Tag
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public IEnumerable<TagDto> Map(IEnumerable<Tag> entities)
        {
            if (entities == null) return null;
            return entities.Select(Map);
        }

        public IEnumerable<Tag> Map(IEnumerable<TagDto> models)
        {
            if (models == null) return null;
            return models.Select(Map);
        }
    }
}