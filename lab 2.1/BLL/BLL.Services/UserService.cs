using BulletinBoard.BLL.Interfaces;
using BulletinBoard.DAL.Models;
using BulletinBoard.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lab_2._1.DAL.Interfaces;

namespace BulletinBoard.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "Користувач не може бути null.");

            // Встановлення дефолтного значення для паролю, якщо він не вказаний
            user.PasswordHash ??= "";

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "Користувач не може бути null.");

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            await _unitOfWork.Users.RemoveByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}