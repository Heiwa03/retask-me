namespace DataAccessLayerCoreTests.MockDbSet
{
    using Microsoft.EntityFrameworkCore;
    using Moq;

    public static class MockDbSet
    {
        public static Mock<DbSet<T>> Create<T>(params T[] elements)
            where T : class
        {
            return elements.AsDbSetMock();
        }
    }
}
