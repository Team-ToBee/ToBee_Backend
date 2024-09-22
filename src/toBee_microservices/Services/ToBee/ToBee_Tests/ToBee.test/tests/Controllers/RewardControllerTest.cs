using AutoMapper;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ToBee.API.Controllers;
using ToBee.API.Dtos.RewardDtos;
using ToBee.API.Models;
using ToBee.API.Repositories.RewardRepository;
using Xunit;

namespace ToBee.API.Tests.Controllers
{
    public class RewardControllerTest
    {
        private readonly Mock<IRewardRepository> _rewardRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly RewardController _controller;

        public RewardControllerTest()
        {
            _rewardRepositoryMock = new Mock<IRewardRepository>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _httpContextAccessorMock.Setup(_ => _.HttpContext.User).Returns(user);

            _controller = new RewardController(_rewardRepositoryMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task GetAllRewardsForUser_ReturnsOkResult_WithListOfRewards()
        {
            // Arrange
            var rewards = new List<Reward> { new Reward() };
            var rewardDtos = new List<RewardDto> { new RewardDto() };
            _rewardRepositoryMock.Setup(repo => repo.GetAllRewardsForUserAsync("test-user-id")).ReturnsAsync(rewards);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<RewardDto>>(rewards)).Returns(rewardDtos);

            // Act
            var result = await _controller.GetAllRewardsForUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<RewardDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetTotalPointsForUser_ReturnsOkResult_WithTotalPoints()
        {
            // Arrange
            var totalPoints = 100;
            _rewardRepositoryMock.Setup(repo => repo.GetTotalPointsForUserAsync("test-user-id")).ReturnsAsync(totalPoints);

            // Act
            var result = await _controller.GetTotalPointsForUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(totalPoints, returnValue);
        }

        [Fact]
        public async Task GetBadgeForUser_ReturnsOkResult_WithBadge()
        {
            // Arrange
            var badge = "Gold";
            _rewardRepositoryMock.Setup(repo => repo.GetBadgeForUserAsync("test-user-id")).ReturnsAsync(badge);

            // Act
            var result = await _controller.GetBadgeForUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal(badge, returnValue);
        }
    }
}