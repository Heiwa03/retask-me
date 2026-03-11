namespace DataAccessLayerCoreTests.MockDbSet
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MockAsyncEnumeratorExtensions<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> inner;

        public MockAsyncEnumeratorExtensions(IEnumerator<T> inner)
           => this.inner = inner ?? throw new ArgumentNullException();

        public T Current
           => this.inner.Current;

        public ValueTask<bool> MoveNextAsync()
         => new ValueTask<bool>(this.inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            this.inner.Dispose();
            return default(ValueTask);
        }
    }
}
