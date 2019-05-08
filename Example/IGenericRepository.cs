using System.Linq;

namespace Example
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> DataSet { get; }
        void AddOrUpdate(T instance);
    }
}