using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayerCore;
using DataAccessLayerCore.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DataAccessLayerCoreTests.Fixtures
{
    public class UserRepositoryFixture : IDisposable
    {
        public Mock<DbSet<User>> MockSet { get; private set; }
        public Mock<DatabaseContext> MockContext { get; private set; }

        public UserRepositoryFixture()
        {
            MockSet = new Mock<DbSet<User>>();
            MockContext = new Mock<DatabaseContext>();

            MockContext.Setup(m => m.Users).Returns(MockSet.Object);
        }

        public void Dispose() { }
    }
}
