using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToBee.API.Data;
using ToBee.API.Models;
using ToBee.API.Repositories.ApplicationUserRepository;
using Xunit;

namespace ToBee.API.Tests.Repos
{
    public class ApplicationUserRepositoryTest
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public ApplicationUserRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new ApplicationUser { Id = userId, UserName = "testuser" };

            using (var context = new AppDbContext(_options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(_options))
            {
                var repository = new ApplicationUserRepository(context);

                // Act
                var result = await repository.GetUserByIdAsync(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userId, result.Id);
                Assert.Equal("testuser", result.UserName);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = "non-existent-user-id";

            using (var context = new AppDbContext(_options))
            {
                var repository = new ApplicationUserRepository(context);

                // Act
                var result = await repository.GetUserByIdAsync(userId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUser()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new ApplicationUser { Id = userId, UserName = "testuser" };

            using (var context = new AppDbContext(_options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(_options))
            {
                var repository = new ApplicationUserRepository(context);
                user.UserName = "updateduser";

                // Act
                await repository.UpdateUserAsync(user);

                // Assert
                var updatedUser = await context.Users.FindAsync(userId);
                Assert.Equal("updateduser", updatedUser.UserName);
            }
        }
    }
}