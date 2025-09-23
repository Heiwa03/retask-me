using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DataAccessLayerCoreTests.Tests
{
    public class UserRepository_InMemory_Tests
    {
        private static DatabaseContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var ctx = new DatabaseContext(options);
            return ctx;
        }

        [Fact]
        public async Task GetUserByEmail_Returns_User_WhenExists()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Users.Add(new User { Uuid = System.Guid.NewGuid(), Username = "user@site.com", NormalizedUsername = "USER@SITE.COM", Password = "hash" });
            await ctx.SaveChangesAsync();

            var repo = new UserRepository(ctx);
            var user = await repo.GetUserByEmail("user@site.com");

            user.Should().NotBeNull();
            user!.NormalizedUsername.Should().Be("USER@SITE.COM");
        }

        [Fact]
        public void IsEmailOccupied_True_WhenEmailExists()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Users.Add(new User { Uuid = System.Guid.NewGuid(), Username = "exist@site.com", NormalizedUsername = "EXIST@SITE.COM", Password = "hash" });
            ctx.SaveChanges();

            var repo = new UserRepository(ctx);
            repo.IsEmailOccupied("exist@site.com").Should().BeTrue();
            repo.IsEmailOccupied("missing@site.com").Should().BeFalse();
        }
    }
}
