using System;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext: IDisposable
    {
        private readonly Action close;
        private readonly Func<TContext> getContext;

        public UnitOfWork(Action close, Func<TContext> getContext)
        {
            this.close = close;
            this.getContext = getContext;
        }

        public virtual void Dispose()
        {
            close();
        }

        private ISaveChanges GetContextWithGuard()
        {
            var context = (getContext() as ISaveChanges);
            if (context == null)
            {
                throw new Exception("Context needs to implement ISaveChanges");
            }

            return context;
        }

        public virtual void SaveChanges()
        {
            ISaveChanges context = GetContextWithGuard();
            context.SaveChanges();
        }

        public virtual async Task SaveChangesAsync()
        {
            ISaveChanges context = GetContextWithGuard();
            await context.SaveChangesAsync();
        }
    }
}
