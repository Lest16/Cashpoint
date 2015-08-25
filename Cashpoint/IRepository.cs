namespace Cashpoint
{
    public interface IRepository<T>
    {
        void Save(T entity);

        T GetContents();

        void Clear();
    }
}
