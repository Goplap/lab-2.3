using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBoard.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var result = new List<UserDto>();
            foreach (var user in users)
            {
                result.Add(MapToDto(user));
            }
            return result;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto, string? password)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = HashPassword(password),
                RegisteredAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var existingUser = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
            if (existingUser == null)
                throw new InvalidOperationException("Користувача не знайдено.");

            existingUser.Username = userDto.Username;
            existingUser.Email = userDto.Email;
            existingUser.IsActive = userDto.IsActive;

            await _unitOfWork.Users.UpdateAsync(existingUser);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            await _unitOfWork.Users.RemoveByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // Приватні допоміжні методи

        private static string HashPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return string.Empty;

            // Простий Base64 — лише для прикладу
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RegisteredAt = user.RegisteredAt,
                IsActive = user.IsActive
            };
        }
    }
}
