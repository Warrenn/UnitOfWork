using System;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        Task SaveChangesAsync();
    }
}
