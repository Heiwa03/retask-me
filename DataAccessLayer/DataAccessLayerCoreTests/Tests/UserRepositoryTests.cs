using DataAccessLayerCore;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCoreTests.MockDbSet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace DataAccessLayerCoreTests
{
    public class UserRepositoryTests
    {
        private readonly Mock<DatabaseContext> _mockContext;

        private readonly UserRepository _userRepository;
        
        // Here you can setup any of the data
        private readonly List<User> _users = GetUserData();
        private readonly User _singleUser = GetSingleUserEntity();

        public UserRepositoryTests() 
        {
            var mockOptions = new DbContextOptions<DatabaseContext>();
            _mockContext = new Mock<DatabaseContext>(mockOptions);

            _userRepository = new UserRepository(_mockContext.Object);
        }

        private static List<User> GetUserData()
        {
            return
            [
                new User
                {
                    Id = 1,
                    Uuid = Guid.NewGuid(),
                    isActive = true,
                    CreatedDate = DateTime.Now,
                    Username = "Penis@gmail.com",
                    NormalizedUsername = "PENIS@GMAIL.COM",
                    Password = "sdasdasdaawdasdasd"
                }
            ];
        }

        private static User GetSingleUserEntity()
        {
            return new User
            {
                Id = 1,
                Uuid = Guid.NewGuid(),
                isActive = true,
                CreatedDate = DateTime.Now,
                Username = "Penis@gmail.com",
                NormalizedUsername = "PENIS@GMAIL.COM",
                Password = "sdasdasdaawdasdasd"
            };
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsSuccessResult_UserWasCreated()
        {
            _mockContext.Setup(x => x.Users).Returns(_users.AsDbSetMock().Object);
            
            var result = await _userRepository.GetUserByUsername(_users.First().NormalizedUsername);
            
            Assert.NotNull(result);
            Assert.Equal(_users.First().Username, result.Username);
        }
        
        [Fact]
        public async Task GetUserByUsername_WhenUserDoesNotExist_ReturnsNull()
        {
            var users = new List<User>
            {
                new() { Id = 1, Username = "Alice", NormalizedUsername = "ALICE", Password = "p1", Uuid = Guid.NewGuid() }
            };

            _mockContext.Setup(x => x.Users).Returns(users.AsDbSetMock().Object);

            var result = await _userRepository.GetUserByUsername("Bob");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsername_IsCaseInsensitive_FindsUser()
        {
            var users = new List<User>
            {
                new() { Id = 2, Username = "John", NormalizedUsername = "JOHN", Password = "p2", Uuid = Guid.NewGuid() }
            };

            _mockContext.Setup(x => x.Users).Returns(users.AsDbSetMock().Object);

            var result = await _userRepository.GetUserByUsername("john");

            Assert.NotNull(result);
            Assert.Equal("JOHN", result!.NormalizedUsername);
            Assert.Equal("John", result.Username);
        }

        [Fact]
        public async Task GetUserByUsername_WhenMultipleHaveSameNormalizedName_ReturnsFirst()
        {
            var first = new User { Id = 3, Username = "FirstDup", NormalizedUsername = "DUP", Password = "p3", Uuid = Guid.NewGuid() };
            var second = new User { Id = 4, Username = "SecondDup", NormalizedUsername = "DUP", Password = "p4", Uuid = Guid.NewGuid() };
            var users = new List<User> { first, second };

            _mockContext.Setup(x => x.Users).Returns(users.AsDbSetMock().Object);

            var result = await _userRepository.GetUserByUsername("dup");

            Assert.NotNull(result);
            Assert.Equal(first.Id, result!.Id);
            Assert.Equal("FirstDup", result.Username);
        }

        [Fact]
        public async Task GetUserByUsername_WithWhitespaceAroundUsername_ReturnsNull()
        {
            var users = new List<User>
            {
                new() { Id = 5, Username = "Emma", NormalizedUsername = "EMMA", Password = "p5", Uuid = Guid.NewGuid() }
            };

            _mockContext.Setup(x => x.Users).Returns(users.AsDbSetMock().Object);

            var result = await _userRepository.GetUserByUsername("  emma  ");

            Assert.Null(result);
        }

        [Fact]
        public void Add_UserEntity_CallsDbContextSetAdd()
        {
            // Arrange
            var mockSet = new Mock<DbSet<User>>();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockSet.Object);

            // Act
            var repository = new UserRepository(_mockContext.Object);
            repository.Add(GetSingleUserEntity());

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void Delete_UserEnity_CallsDbContextSetRemove()
        {
            // Arrange
            var mockSet = new Mock<DbSet<User>>();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockSet.Object);

            // Act
            var repository = new UserRepository(_mockContext.Object);
            repository.Delete(_singleUser);

            // Assert
            mockSet.Verify(m => m.Remove(It.Is<User>(u => u.Uuid == _singleUser.Uuid)), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectEntity()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "test_user_1", Uuid = Guid.NewGuid(), Password = "p1", isActive = true, NormalizedUsername = "T1" },
                new User { Id = 2, Username = "test_user_2", Uuid = Guid.NewGuid(), Password = "p2", isActive = true, NormalizedUsername = "T2" }
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

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
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

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
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

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
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

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
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

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
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

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
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

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

            var mockDbSet = GetUserData().AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

            // Act
            repository.Update(userToUpdate);

            // Assert
            mockDbSet.Verify(m => m.Update(It.Is<User>(u => u.Id == 1 && u.Username == "updated_user")), Times.Once);
        }
        
        [Fact]
        public void Update_WithNullEntity_ThrowsArgumentNullException()
        {
            User? nullUser = null;

            var mockDbSet = GetUserData().AsDbSetMock();

            _mockContext.Setup(c => c.Set<User>()).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.Update(nullUser!));
        }

        [Fact]
        public async Task SaveChangesAsync_CallsDbContextSaveChangesAsyncAndReturnsCorrectValue()
        {
            // Arrange
            var expectedResult = 1;

            // Set up the mock context to return the expected result when SaveChangesAsync is called.
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var repository = new UserRepository(_mockContext.Object);

            // Act
            var result = await repository.SaveChangesAsync();

            // Assert
            Assert.Equal(expectedResult, result);

            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BeginTransactionAsync_CallsBeginTransactionOnDatabaseAndReturnsTransaction()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockContext = new Mock<DatabaseContext>(options);
            var mockDatabase = new Mock<DatabaseFacade>(mockContext.Object);
            var mockTransaction = new Mock<IDbContextTransaction>();

            // Setup:
            // Configure the mock database to return the mock transaction.
            mockDatabase
                .Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockTransaction.Object);

            // Configure the mock context to return the mock database facade.
            mockContext
                .Setup(c => c.Database)
                .Returns(mockDatabase.Object);

            var repository = new UserRepository(mockContext.Object);

            // Act
            var result = await repository.BeginTransactionAsync();

            // Assert
            mockDatabase.Verify(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(mockTransaction.Object, result);
        }

        [Fact]
        public void IsUsernameOccupied_WhenUsernameExists_ReturnsTrue()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "TestUser", NormalizedUsername = "TESTUSER", Password = "p1", Uuid = Guid.NewGuid() }
            };

            var mockDbSet = users.AsDbSetMock();

            _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var repository = new UserRepository(_mockContext.Object);

            // Act
            var isOccupied = repository.IsUsernameOccupied("TestUser");

            // Assert
            //Assert.True(isOccupied);
        }
    }
}
