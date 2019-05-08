using System;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public partial class UnitOfWorkManager
    {
        public class UnitOfWork : IUnitOfWork
        {
            private readonly Action popOffStack;
            private readonly Func<Task> saveAsync;

            public UnitOfWork(Action popOffStack, Func<Task> saveAsync)
            {
                this.popOffStack = popOffStack;
                this.saveAsync = saveAsync;
            }


            public void Dispose()
            {
                popOffStack();
            }

            public async Task SaveChangesAsync()
            {
                await saveAsync();
            }
        }
    }
}
