using AutoFixture;
using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;
using BulletinBoard.BLL.Services;
using BulletinBoard.DAL.Models;
using lab_2._1.DAL.Interfaces;
using NSubstitute;
using Xunit;

namespace BulletinBoard.BLL.Tests.Services
{
    public class AdServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<Ad, AdDto> _mapper;
        private readonly AdService _adService;
        private readonly Fixture _fixture;

        public AdServiceTests()
        {
            // Setup NSubstitute mocks
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper<Ad, AdDto>>();
            _adService = new AdService(_unitOfWork, _mapper);

            // Setup AutoFixture
            _fixture = new Fixture();
            // Configure AutoFixture to handle recursive references if needed
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAllAdsAsync_ShouldReturnAllAds()
        {
            // Arrange
            var ads = _fixture.CreateMany<Ad>(3).ToList();
            var adDtos = _fixture.CreateMany<AdDto>(3).ToList();

            _unitOfWork.Ads.GetAllAsync().Returns(ads);
            _mapper.Map(Arg.Any<IEnumerable<Ad>>()).Returns(adDtos);

            // Act
            var result = await _adService.GetAllAdsAsync();

            // Assert
            await _unitOfWork.Ads.Received(1).GetAllAsync();
            _mapper.Received(1).Map(Arg.Is<IEnumerable<Ad>>(x => x == ads));
            Assert.Equal(adDtos, result);
        }

        [Fact]
        public async Task GetAdByIdAsync_ShouldReturnAdDto()
        {
            // Arrange
            int adId = 1;
            var ad = _fixture.Create<Ad>();
            var adDto = _fixture.Create<AdDto>();

            _unitOfWork.Ads.GetByIdAsync(adId).Returns(ad);
            _mapper.Map(Arg.Any<Ad>()).Returns(adDto);

            // Act
            var result = await _adService.GetAdByIdAsync(adId);

            // Assert
            await _unitOfWork.Ads.Received(1).GetByIdAsync(adId);
            _mapper.Received(1).Map(Arg.Is<Ad>(x => x == ad));
            Assert.Equal(adDto, result);
        }

        [Fact]
        public async Task GetAdsByUserIdAsync_ShouldReturnUserAds()
        {
            // Arrange
            int userId = 1;
            var ads = _fixture.CreateMany<Ad>(2).ToList();
            var adDtos = _fixture.CreateMany<AdDto>(2).ToList();

            _unitOfWork.Ads.GetAdsByUserIdAsync(userId).Returns(ads);
            _mapper.Map(Arg.Any<IEnumerable<Ad>>()).Returns(adDtos);

            // Act
            var result = await _adService.GetAdsByUserIdAsync(userId);

            // Assert
            await _unitOfWork.Ads.Received(1).GetAdsByUserIdAsync(userId);
            _mapper.Received(1).Map(Arg.Is<IEnumerable<Ad>>(x => x == ads));
            Assert.Equal(adDtos, result);
        }

        [Fact]
        public async Task GetAdsByCategoryIdAsync_ShouldReturnCategoryAds()
        {
            // Arrange
            int categoryId = 1;
            var ads = _fixture.CreateMany<Ad>(2).ToList();
            var adDtos = _fixture.CreateMany<AdDto>(2).ToList();

            _unitOfWork.Ads.GetAdsByCategoryIdAsync(categoryId).Returns(ads);
            _mapper.Map(Arg.Any<IEnumerable<Ad>>()).Returns(adDtos);

            // Act
            var result = await _adService.GetAdsByCategoryIdAsync(categoryId);

            // Assert
            await _unitOfWork.Ads.Received(1).GetAdsByCategoryIdAsync(categoryId);
            _mapper.Received(1).Map(Arg.Is<IEnumerable<Ad>>(x => x == ads));
            Assert.Equal(adDtos, result);
        }

        [Fact]
        public async Task CreateAdAsync_WithValidData_ShouldCreateAndReturnAd()
        {
            // Arrange
            var adDto = _fixture.Create<AdDto>();
            var ad = _fixture.Create<Ad>();
            var category = _fixture.Create<Category>();
            var user = _fixture.Create<User>();

            _unitOfWork.Categories.GetByIdAsync(adDto.CategoryId).Returns(category);
            _unitOfWork.Users.GetByIdAsync(adDto.UserId).Returns(user);
            _mapper.Map(Arg.Any<AdDto>()).Returns(ad);

            // Act
            var result = await _adService.CreateAdAsync(adDto);

            // Assert
            await _unitOfWork.Categories.Received(1).GetByIdAsync(adDto.CategoryId);
            await _unitOfWork.Users.Received(1).GetByIdAsync(adDto.UserId);
            _mapper.Received(1).Map(Arg.Is<AdDto>(x => x == adDto));
            await _unitOfWork.Ads.Received(1).AddAsync(ad);
            await _unitOfWork.Received(1).SaveChangesAsync();
            Assert.Equal(adDto, result);
        }

        [Fact]
        public async Task CreateAdAsync_WithNullAd_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _adService.CreateAdAsync(null));

            Assert.Equal("adDto", exception.ParamName);
        }

        [Fact]
        public async Task CreateAdAsync_WithNonexistentCategory_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var adDto = _fixture.Create<AdDto>();
            _unitOfWork.Categories.GetByIdAsync(adDto.CategoryId).Returns((Category)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _adService.CreateAdAsync(adDto));

            Assert.Contains($"Категорія з ID {adDto.CategoryId} не існує", exception.Message);
        }

        [Fact]
        public async Task CreateAdAsync_WithNonexistentUser_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var adDto = _fixture.Create<AdDto>();
            var category = _fixture.Create<Category>();

            _unitOfWork.Categories.GetByIdAsync(adDto.CategoryId).Returns(category);
            _unitOfWork.Users.GetByIdAsync(adDto.UserId).Returns((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _adService.CreateAdAsync(adDto));

            Assert.Contains($"Користувач з ID {adDto.UserId} не існує", exception.Message);
        }

        [Fact]
        public async Task UpdateAdAsync_WithValidData_ShouldUpdateAd()
        {
            // Arrange
            var adDto = _fixture.Create<AdDto>();
            var ad = _fixture.Create<Ad>();

            _mapper.Map(Arg.Any<AdDto>()).Returns(ad);

            // Act
            await _adService.UpdateAdAsync(adDto);

            // Assert
            _mapper.Received(1).Map(Arg.Is<AdDto>(x => x == adDto));
            await _unitOfWork.Ads.Received(1).UpdateAsync(ad);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateAdAsync_WithNullAd_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _adService.UpdateAdAsync(null));

            Assert.Equal("adDto", exception.ParamName);
        }

        [Fact]
        public async Task DeleteAdAsync_ShouldDeleteAd()
        {
            // Arrange
            int adId = 1;

            // Act
            await _adService.DeleteAdAsync(adId);

            // Assert
            await _unitOfWork.Ads.Received(1).RemoveByIdAsync(adId);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }
    }
}