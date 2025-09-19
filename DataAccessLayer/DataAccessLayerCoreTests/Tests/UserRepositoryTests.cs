using DataAccessLayerCore;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCoreTests.MockDbSet;
using Microsoft.EntityFrameworkCore;
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
        public void Add_UserEntity_CallsDbContextSetAdd()
        {
            // Arrange
            var users = new List<User> { };

            _mockContext.Setup(c => c.Set<User>()).Returns(users.AsDbSetMock().Object);

            // Act
            var repository = new UserRepository(_mockContext.Object);
            repository.Add(_singleUser);

            // Assert
            users.AsDbSetMock().Verify(m => m.Add(It.Is<User>(u => u.Username == GetSingleUserEntity().Username)), Times.Once);
        }
        
        [Fact]
        public void Delete_UserEnity_CallsDbContextSetRemove()
        {
            // Arrange
            var users = GetUserData();

            _mockContext.Setup(c => c.Set<User>()).Returns(users.AsDbSetMock().Object);

            // Act
            var repository = new UserRepository(_mockContext.Object);
            repository.Delete(_singleUser);

            // Assert
            users.AsDbSetMock().Verify(m => m.Remove(It.Is<User>(u => u.Uuid == _singleUser.Uuid)), Times.Once);
        }
        /*
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

        [Fact]
        public void IsUserNameOccupied_WhenUsernameExists_ReturnsTrue()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "TestUser", NormalizedUsername = "TESTUSER", Password = "p1", Uuid = Guid.NewGuid() }
            }.AsQueryable();

            var repository = new UserRepository(_fixture.MockContext.Object);

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Act
            var isOccupied = repository.IsUserNameOccupied("TestUser");

            // Assert
            Assert.True(isOccupied);
        }
        
        [Fact]
        public void IsUserNameOccupied_WhenUsernameDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "ExistingUser", NormalizedUsername = "EXISTINGUSER", Password = "p1", Uuid = Guid.NewGuid() }
            }.AsQueryable();

            var repository = new UserRepository(_fixture.MockContext.Object);

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Act
            var isOccupied = repository.IsUserNameOccupied("NonExistentUser");

            // Assert
            Assert.False(isOccupied);
        }

        [Fact]
        public async Task GetUserByUsername_WhenUsernameExists_ReturnsUser()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "TestUser", NormalizedUsername = "TESTUSER", Password = "p1", Uuid = Guid.NewGuid() }
            }.AsQueryable();

            // Configure the DbSet mock for asynchronous LINQ operations.
            // This is the correct syntax for chaining the setup.
            _fixture.MockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));

            _fixture.MockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(users.Provider));

            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _fixture.MockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _fixture.MockContext.Setup(m => m.Users).Returns(_fixture.MockSet.Object);

            var repository = new UserRepository(_fixture.MockContext.Object);

            // Act
            var result = await repository.GetUserByUsername("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TESTUSER", result.NormalizedUsername);
        }*/
    }
}
