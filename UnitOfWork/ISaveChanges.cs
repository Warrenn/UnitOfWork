using System.Threading.Tasks;

namespace UnitOfWork
{
    public interface ISaveChanges
    {
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
