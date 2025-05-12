using AutoFixture;
using BulletinBoard.BLL.Models;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using NSubstitute;
using Xunit;

namespace BulletinBoard.BLL.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserService _userService;
        private readonly Fixture _fixture;

        public UserServiceTests()
        {
            // Setup NSubstitute mocks
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _userService = new UserService(_unitOfWork);

            // Setup AutoFixture
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUserDtos()
        {
            // Arrange
            var users = _fixture.CreateMany<User>(3).ToList();
            _unitOfWork.Users.GetAllAsync().Returns(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            await _unitOfWork.Users.Received(1).GetAllAsync();
            Assert.Equal(users.Count, result.Count());
            // Additional assertions to verify mapping from User to UserDto
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserDto()
        {
            // Arrange
            int userId = 1;
            var user = _fixture.Create<User>();
            _unitOfWork.Users.GetByIdAsync(userId).Returns(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            await _unitOfWork.Users.Received(1).GetByIdAsync(userId);
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            // Additional assertions to verify other properties are mapped correctly
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUserDto()
        {
            // Arrange
            string email = "test@example.com";
            var user = _fixture.Create<User>();
            _unitOfWork.Users.GetByEmailAsync(email).Returns(user);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            await _unitOfWork.Users.Received(1).GetByEmailAsync(email);
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            // Additional assertions to verify other properties are mapped correctly
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldCreateAndReturnUserDto()
        {
            // Arrange
            var userDto = _fixture.Create<UserDto>();
            string password = "TestPassword123!";

            // Setup the mock behavior for entity creation
            _unitOfWork.Users.AddAsync(Arg.Any<User>())
                .Returns(callInfo =>
                {
                    // Return the User that was passed to AddAsync
                    return Task.FromResult(callInfo.Arg<User>());
                });

            // Act
            var result = await _userService.CreateUserAsync(userDto, password);

            // Assert
            await _unitOfWork.Users.Received(1).AddAsync(Arg.Is<User>(u =>
                u.Email == userDto.Email &&
                !string.IsNullOrEmpty(u.PasswordHash)));

            await _unitOfWork.Received(1).SaveChangesAsync();
            Assert.NotNull(result);
            Assert.Equal(userDto.Email, result.Email);
            // Additional assertions to verify other properties are mapped correctly
        }

        [Fact]
        public async Task CreateUserAsync_WithNullUserDto_ShouldThrowArgumentNullException()
        {
            // Arrange
            string password = "TestPassword123!";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _userService.CreateUserAsync(null, password));

            Assert.Equal("userDto", exception.ParamName);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidData_ShouldUpdateUser()
        {
            // Arrange
            var userDto = _fixture.Create<UserDto>();
            var existingUser = _fixture.Create<User>();

            _unitOfWork.Users.GetByIdAsync(userDto.Id).Returns(existingUser);

            // Act
            await _userService.UpdateUserAsync(userDto);

            // Assert
            await _unitOfWork.Users.Received(1).UpdateAsync(Arg.Is<User>(u =>
                u.Id == userDto.Id &&
                u.Email == userDto.Email));

            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserAsync_WithNullUserDto_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _userService.UpdateUserAsync(null));

            Assert.Equal("userDto", exception.ParamName);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            // Arrange
            int userId = 1;

            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            await _unitOfWork.Users.Received(1).RemoveByIdAsync(userId);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }
    }
}