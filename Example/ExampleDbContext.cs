using Microsoft.EntityFrameworkCore;
using UnitOfWork;
using System.Threading.Tasks;

namespace Example
{
    public class ExampleDbContext : DbContext, ISaveChanges
    {
        public ExampleDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UserEntity> Users { get; set; }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        void ISaveChanges.SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
