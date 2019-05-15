using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using System;
using System.Collections.Generic;
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

            container.Register<IDataContextManager<ExampleDbContext>>(() => new DataContextManager<ExampleDbContext>(
                () => new ExampleDbContext(optionsBuilder.Options)));

            container.Register(typeof(IGenericRepository<>), typeof(EFGenericRepositoryExample<>));
            container.Register<ExampleController>();

            var manager = container.GetInstance<IDataContextManager<ExampleDbContext>>();
            var ctx = manager.CurrentContext;

            var controller = container.GetInstance<ExampleController>();
            await controller.ExampleUseUnitOfWork();


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
