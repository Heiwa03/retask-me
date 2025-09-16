using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCoreTests.Fixtures;
using DataAccessLayerCoreTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace DataAccessLayerCoreTests
{
    public class UserRepositoryTests : IClassFixture<UserRepositoryFixture>
    {
        private readonly UserRepositoryFixture _fixture;

        public UserRepositoryTests (UserRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Add_AddEntity_CallsDbContextSetAdd()
        {
            // Arrange
            var testUser = new User
            {
                Uuid = Guid.NewGuid(),
                Username = "ivan.revenko@isa.utm.md",
                NormalizedUsername = "IVAN.REVENKO@ISA.UTM.MD",
                Password = "password123"
            };

            _fixture.MockContext.Setup(c => c.Set<User>()).Returns(_fixture.MockSet.Object);

            // Act
            var repository = new UserRepository(_fixture.MockContext.Object);
            repository.Add(testUser);

            // Assert
            _fixture.MockSet.Verify(m => m.Add(It.Is<User>(u => u.Username == "ivan.revenko@isa.utm.md")), Times.Once);
        }

        [Fact]
        public void Delete_UserEnity_CallsDbContextSetRemove()
        {
            // Arrange
            var testUser = new User
            {
                Uuid = Guid.NewGuid(),
                Username = "ivan.revenko@isa.utm.md",
                NormalizedUsername = "IVAN.REVENKO@ISA.UTM.MD",
                Password = "password123"
            };

            _fixture.MockContext.Setup(c => c.Set<User>()).Returns(_fixture.MockSet.Object);

            // Act
            var repository = new UserRepository(_fixture.MockContext.Object);
            repository.Delete(testUser);

            // Assert
            _fixture.MockSet.Verify(m => m.Remove(It.Is<User>(u => u.Uuid == testUser.Uuid)), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectEntity()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "test_user_1", Uuid = Guid.NewGuid(), Password = "p1", isActive = true, NormalizedUsername = "T1" },
                new User { Id = 2, Username = "test_user_2", Uuid = Guid.NewGuid(), Password = "p2", isActive = true, NormalizedUsername = "T2" }
            }.AsQueryable();

            _fixture.MockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));

            _fixture.MockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(users.Provider));

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = await repository.GetByIdAsync<User>(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("test_user_2", result.Username);
        }

        [Fact]
        public void GetById_WithValidId_ReturnsCorrectEntity()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "test_user_1", Uuid = Guid.NewGuid(), Password = "p1", NormalizedUsername = "T1" },
                new User { Id = 2, Username = "test_user_2", Uuid = Guid.NewGuid(), Password = "p2", NormalizedUsername = "T2" }
            }.AsQueryable();

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = repository.GetById<User>(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("test_user_2", result.Username);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "test_user_1", Uuid = Guid.NewGuid(), Password = "p1", NormalizedUsername = "T1" }
            }.AsQueryable();

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = repository.GetById<User>(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUuidAsync_WithValidUuid_ReturnsCorrectEntity()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = 1, Uuid = testGuid, Username = "test_user_1", Password = "p1", NormalizedUsername = "T1" },
                new User { Id = 2, Uuid = Guid.NewGuid(), Username = "test_user_2", Password = "p2", NormalizedUsername = "T2" }
            }.AsQueryable();

            // Use the fixture's MockSet to configure async LINQ operations
            _fixture.MockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));

            _fixture.MockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(users.Provider));

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = await repository.GetByUuidAsync<User>(testGuid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testGuid, result.Uuid);
            Assert.Equal("test_user_1", result.Username);
        }

        [Fact]
        public async Task GetByUuidAsync_WithInvalidUuid_ReturnsNull()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Uuid = Guid.NewGuid(), Username = "test_user_1", Password = "p1", NormalizedUsername = "T1" }
            }.AsQueryable();

            _fixture.MockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(users.Provider));
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = await repository.GetByUuidAsync<User>(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetByUuid_WithValidUuid_ReturnsCorrectEntity()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = 1, Uuid = testGuid, Username = "test_user_1", Password = "p1", NormalizedUsername = "T1" },
                new User { Id = 2, Uuid = Guid.NewGuid(), Username = "test_user_2", Password = "p2", NormalizedUsername = "T2" }
            }.AsQueryable();

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = repository.GetByUuid<User>(testGuid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testGuid, result.Uuid);
            Assert.Equal("test_user_1", result.Username);
        }

        [Fact]
        public void GetByUuid_WithInvalidUuid_ReturnsNull()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Uuid = Guid.NewGuid(), Username = "test_user_1", Password = "p1", NormalizedUsername = "T1" }
            }.AsQueryable();

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = repository.GetByUuid<User>(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Update_UserEntity_CallsDbContextSetUpdate()
        {
            // Arrange
            var userToUpdate = new User
            {
                Id = 1,
                Uuid = Guid.NewGuid(),
                Username = "updated_user",
                NormalizedUsername = "UPDATED_USER",
                Password = "new_password"
            };

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            repository.Update(userToUpdate);

            // Assert
            // Verify that the Update method was called on the DbSet mock with the correct user entity.
            _fixture.MockSet.Verify(m => m.Update(It.Is<User>(u => u.Id == 1 && u.Username == "updated_user")), Times.Once);
        }

        [Fact]
        public void Update_WithNullEntity_ThrowsArgumentNullException()
        {
            User? nullUser = null;
            var repository = new BaseRepository(_fixture.MockContext.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.Update(nullUser!));
        }

        
        // I know this looks useless but let it be here
        [Fact]
        public async Task SaveChangesAsync_CallsDbContextSaveChangesAsyncAndReturnsCorrectValue()
        {
            // Arrange
            var expectedResult = 1;

            // Set up the mock context to return the expected result when SaveChangesAsync is called.
            _fixture.MockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = await repository.SaveChangesAsync();

            // Assert
            // 1. Verify that the returned value is the one we set up.
            Assert.Equal(expectedResult, result);

            // 2. Verify that SaveChangesAsync was called exactly once on the mock context.
            _fixture.MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BeginTransactionAsync_CallsBeginTransactionOnDatabaseAndReturnsTransaction()
        {
            // Arrange
            // Mock the IDbContextTransaction object that is expected to be returned.
            var mockTransaction = new Mock<IDbContextTransaction>();

            // Mock the Database property (DatabaseFacade) of the DbContext.
            var mockDatabase = new Mock<DatabaseFacade>(_fixture.MockContext.Object);

            // Set up the mock DatabaseFacade to return the mock transaction when BeginTransactionAsync is called.
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockTransaction.Object);

            // Set up the mock DbContext (from the fixture) to return the mock DatabaseFacade.
            _fixture.MockContext.Setup(c => c.Database).Returns(mockDatabase.Object);

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = await repository.BeginTransactionAsync();

            // Assert
            // 1. Verify that BeginTransactionAsync was called on the mock DatabaseFacade.
            mockDatabase.Verify(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);

            // 2. Verify that the returned object is the same mock transaction that was set up.
            Assert.Equal(mockTransaction.Object, result);
        }
    }
}
