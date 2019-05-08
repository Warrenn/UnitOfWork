using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using System;
using System.Threading.Tasks;
using UnitOfWork;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var container = new Container();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("InMemory");

            container.Register<IUnitOfWorkManager>(() => new UnitOfWorkManager(
                () => new ExampleDbContext(optionsBuilder.Options),
                c => (c as ExampleDbContext).SaveChangesAsync()));

            container.Register(typeof(IGenericRepository<>), typeof(EFGenericRepositoryExample<>));
            container.Register<ExampleController>();

            var controller = container.GetInstance<ExampleController>();
            await controller.ExampleUseUnitOfWork();

            var manager = container.GetInstance<IUnitOfWorkManager>();
            var ctx = manager.CurrentContext as ExampleDbContext;

            Console.WriteLine("Done now looping through all of them should see 2 both id 1 and id 2");
            foreach (var user in ctx.Users)
            {
                Console.WriteLine(user.Id);
                Console.WriteLine(user.Name);
                Console.WriteLine(user.Date);
            }

            Console.ReadLine();
        }
    }
}
