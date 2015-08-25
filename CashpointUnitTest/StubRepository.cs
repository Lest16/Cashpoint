namespace CashpointUnitTest
{

    using Cashpoint;

    public class StubRepository<T> : IRepository<T>
    {
        public void Save(T entity)
        {
        }

        public T GetContents()
        {
            return (T)new object();
        }

        public void Clear()
        {
        }
    }
}