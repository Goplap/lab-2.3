using BulletinBoard.BLL.Interfaces;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _unitOfWork.Tags.GetAllAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await _unitOfWork.Tags.GetByIdAsync(id);
        }

        public async Task<Tag> GetTagByNameAsync(string name)
        {
            return await _unitOfWork.Tags.GetByNameAsync(name);
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag), "Тег не може бути null.");

            await _unitOfWork.Tags.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            return tag;
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag), "Тег не може бути null.");

            await _unitOfWork.Tags.UpdateAsync(tag);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(int id)
        {
            await _unitOfWork.Tags.RemoveByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}