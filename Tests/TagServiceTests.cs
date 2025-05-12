using AutoFixture;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using NSubstitute;
using Xunit;

namespace BulletinBoard.BLL.Tests.Services
{
    public class TagServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TagService _tagService;
        private readonly Fixture _fixture;

        public TagServiceTests()
        {
            // Setup NSubstitute mocks
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _tagService = new TagService(_unitOfWork);

            // Setup AutoFixture
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAllTagsAsync_ShouldReturnAllTags()
        {
            // Arrange
            var tags = _fixture.CreateMany<Tag>(3).ToList();
            _unitOfWork.Tags.GetAllAsync().Returns(tags);

            // Act
            var result = await _tagService.GetAllTagsAsync();

            // Assert
            await _unitOfWork.Tags.Received(1).GetAllAsync();
            Assert.Equal(tags, result);
        }

        [Fact]
        public async Task GetTagByIdAsync_ShouldReturnTag()
        {
            // Arrange
            int tagId = 1;
            var tag = _fixture.Create<Tag>();
            _unitOfWork.Tags.GetByIdAsync(tagId).Returns(tag);

            // Act
            var result = await _tagService.GetTagByIdAsync(tagId);

            // Assert
            await _unitOfWork.Tags.Received(1).GetByIdAsync(tagId);
            Assert.Equal(tag, result);
        }

        [Fact]
        public async Task GetTagByNameAsync_ShouldReturnTag()
        {
            // Arrange
            string tagName = "test-tag";
            var tag = _fixture.Create<Tag>();
            _unitOfWork.Tags.GetByNameAsync(tagName).Returns(tag);

            // Act
            var result = await _tagService.GetTagByNameAsync(tagName);

            // Assert
            await _unitOfWork.Tags.Received(1).GetByNameAsync(tagName);
            Assert.Equal(tag, result);
        }

        [Fact]
        public async Task CreateTagAsync_WithValidData_ShouldCreateAndReturnTag()
        {
            // Arrange
            var tag = _fixture.Create<Tag>();

            // Act
            var result = await _tagService.CreateTagAsync(tag);

            // Assert
            await _unitOfWork.Tags.Received(1).AddAsync(tag);
            await _unitOfWork.Received(1).SaveChangesAsync();
            Assert.Equal(tag, result);
        }

        [Fact]
        public async Task CreateTagAsync_WithNullTag_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _tagService.CreateTagAsync(null));

            Assert.Equal("tag", exception.ParamName);
        }

        [Fact]
        public async Task UpdateTagAsync_WithValidData_ShouldUpdateTag()
        {
            // Arrange
            var tag = _fixture.Create<Tag>();

            // Act
            await _tagService.UpdateTagAsync(tag);

            // Assert
            await _unitOfWork.Tags.Received(1).UpdateAsync(tag);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateTagAsync_WithNullTag_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _tagService.UpdateTagAsync(null));

            Assert.Equal("tag", exception.ParamName);
        }

        [Fact]
        public async Task DeleteTagAsync_ShouldDeleteTag()
        {
            // Arrange
            int tagId = 1;

            // Act
            await _tagService.DeleteTagAsync(tagId);

            // Assert
            await _unitOfWork.Tags.Received(1).RemoveByIdAsync(tagId);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }
    }
}