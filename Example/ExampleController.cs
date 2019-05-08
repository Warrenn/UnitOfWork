using System;
using System.Threading.Tasks;
using UnitOfWork;

namespace Example
{
    public class ExampleController
    {
        private readonly IUnitOfWorkManager manager;
        private readonly IGenericRepository<UserEntity> repository;

        public ExampleController(IUnitOfWorkManager manager, IGenericRepository<UserEntity> repository)
        {
            this.manager = manager;
            this.repository = repository;
        }

        public async Task ExampleUseUnitOfWork()
        {
            using (var unitOfWork = manager.Create())
            {
                var entity = new UserEntity { Id = 1, Date = DateTime.Now, Name = "test" };
                repository.AddOrUpdate(entity);

                using(var nestedUnitOfWork = manager.Create())
                {
                    var entity2 = new UserEntity { Id = 2, Date = DateTime.Now, Name = "test2" };
                    repository.AddOrUpdate(entity2);
                    await nestedUnitOfWork.SaveChangesAsync();
                }

                foreach(var u in repository.DataSet)
                {
                    Console.WriteLine("should be for id == 2");
                    Console.WriteLine(u.Id);
                    Console.WriteLine(u.Name);
                    Console.WriteLine(u.Date);
                    Console.WriteLine("should not see anymore till done");
                }

                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}