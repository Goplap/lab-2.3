using AutoFixture;
using BulletinBoard.BLL.Models;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using NSubstitute;
using Xunit;

namespace BulletinBoard.BLL.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CategoryService _categoryService;
        private readonly Fixture _fixture;

        public CategoryServiceTests()
        {
            // Setup NSubstitute mocks
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _categoryService = new CategoryService(_unitOfWork);

            // Setup AutoFixture
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
        {
            // Arrange
            var dalCategories = _fixture.CreateMany<Category>(3).ToList();
            _unitOfWork.Categories.GetAllAsync().Returns(dalCategories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            await _unitOfWork.Categories.Received(1).GetAllAsync();
            Assert.Equal(dalCategories.Count, result.Count());

            // Verify each item is converted to a CategoryDto
            foreach (var dalCategory in dalCategories)
            {
                Assert.Contains(result, dto =>
                    dto.Id == dalCategory.Id &&
                    dto.Name == dalCategory.Name);
            }
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory()
        {
            // Arrange
            int categoryId = 1;
            var dalCategory = _fixture.Create<Category>();
            dalCategory.Id = categoryId;
            _unitOfWork.Categories.GetByIdAsync(categoryId).Returns(dalCategory);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            await _unitOfWork.Categories.Received(1).GetByIdAsync(categoryId);
            Assert.NotNull(result);
            Assert.Equal(dalCategory.Id, result.Id);
            Assert.Equal(dalCategory.Name, result.Name);
        }

        [Fact]
        public async Task GetCategoriesWithSubcategoriesAsync_ShouldReturnCategoriesWithSubcategories()
        {
            // Arrange
            var dalCategories = _fixture.CreateMany<Category>(3).ToList();
            _unitOfWork.Categories.GetCategoriesWithSubcategoriesAsync().Returns(dalCategories);

            // Act
            var result = await _categoryService.GetCategoriesWithSubcategoriesAsync();

            // Assert
            await _unitOfWork.Categories.Received(1).GetCategoriesWithSubcategoriesAsync();
            Assert.Equal(dalCategories.Count, result.Count());

            // Verify each item is converted to a CategoryDto
            foreach (var dalCategory in dalCategories)
            {
                Assert.Contains(result, dto =>
                    dto.Id == dalCategory.Id &&
                    dto.Name == dalCategory.Name);
            }
        }

        [Fact]
        public async Task CreateCategoryAsync_WithValidData_ShouldCreateAndReturnCategory()
        {
            // Arrange
            var categoryDto = _fixture.Create<CategoryDto>();
            categoryDto.ParentCategoryId = null; // No parent category

            var dalCategory = new Category
            {
                Name = categoryDto.Name,
                ParentCategoryId = null
            };

            // Mock the AddAsync method to capture the saved category
            Category savedCategory = null;
            _unitOfWork.Categories.AddAsync(Arg.Do<Category>(c => savedCategory = c))
                .Returns(Task.CompletedTask);

            // After saving, we should return the id
            savedCategory = new Category
            {
                Id = 1,
                Name = categoryDto.Name
            };

            // Act
            var result = await _categoryService.CreateCategoryAsync(categoryDto);

            // Assert
            await _unitOfWork.Categories.Received(1).AddAsync(Arg.Any<Category>());
            await _unitOfWork.Received(1).SaveChangesAsync();

            Assert.NotNull(result);
            Assert.Equal(savedCategory.Id, result.Id);
            Assert.Equal(categoryDto.Name, result.Name);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithNullCategory_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _categoryService.CreateCategoryAsync(null));

            Assert.Equal("categoryDto", exception.ParamName);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithValidParentCategory_ShouldCreateAndReturnCategory()
        {
            // Arrange
            var parentCategoryId = 1;
            var parentCategory = _fixture.Create<Category>();
            parentCategory.Id = parentCategoryId;

            var categoryDto = _fixture.Create<CategoryDto>();
            categoryDto.ParentCategoryId = parentCategoryId;

            _unitOfWork.Categories.GetByIdAsync(parentCategoryId).Returns(parentCategory);

            // Mock the AddAsync method to capture the saved category
            Category savedCategory = null;
            _unitOfWork.Categories.AddAsync(Arg.Do<Category>(c => savedCategory = c))
                .Returns(Task.CompletedTask);

            // After saving, we should return the id
            savedCategory = new Category
            {
                Id = 2,
                Name = categoryDto.Name,
                ParentCategoryId = parentCategoryId
            };

            // Act
            var result = await _categoryService.CreateCategoryAsync(categoryDto);

            // Assert
            await _unitOfWork.Categories.Received(1).GetByIdAsync(parentCategoryId);
            await _unitOfWork.Categories.Received(1).AddAsync(Arg.Any<Category>());
            await _unitOfWork.Received(1).SaveChangesAsync();

            Assert.NotNull(result);
            Assert.Equal(savedCategory.Id, result.Id);
            Assert.Equal(categoryDto.Name, result.Name);
            Assert.Equal(parentCategoryId, result.ParentCategoryId);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithNonExistentParentCategory_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var categoryDto = _fixture.Create<CategoryDto>();
            categoryDto.ParentCategoryId = 999; // Non-existent parent category

            _unitOfWork.Categories.GetByIdAsync(categoryDto.ParentCategoryId.Value).Returns((Category)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _categoryService.CreateCategoryAsync(categoryDto));

            Assert.Contains($"Батьківська категорія з ID {categoryDto.ParentCategoryId}", exception.Message);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithValidData_ShouldUpdateCategory()
        {
            // Arrange
            var categoryDto = _fixture.Create<CategoryDto>();
            var existingCategory = _fixture.Create<Category>();
            existingCategory.Id = categoryDto.Id;

            _unitOfWork.Categories.GetByIdAsync(categoryDto.Id).Returns(existingCategory);

            // Act
            await _categoryService.UpdateCategoryAsync(categoryDto);

            // Assert
            await _unitOfWork.Categories.Received(1).UpdateAsync(Arg.Any<Category>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithNullCategory_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _categoryService.UpdateCategoryAsync(null));

            Assert.Equal("categoryDto", exception.ParamName);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldDeleteCategory()
        {
            // Arrange
            int categoryId = 1;

            // Act
            await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            await _unitOfWork.Categories.Received(1).RemoveByIdAsync(categoryId);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }
    }
}