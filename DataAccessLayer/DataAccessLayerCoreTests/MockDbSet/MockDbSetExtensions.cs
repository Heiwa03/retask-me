namespace DataAccessLayerCoreTests.MockDbSet
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Moq;

    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> AsDbSetMock<T>(this IEnumerable<T> list)
            where T : class
        {
            IQueryable<T> queryableList = list.AsQueryable();
            Mock<DbSet<T>> dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(default))
            .Returns(new MockAsyncEnumeratorExtensions<T>(queryableList.GetEnumerator()));

            dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new MockAsyncDbSetExtensions<T>(queryableList.Provider));

            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(queryableList.GetEnumerator());

            return dbSetMock;
        }
    }
}